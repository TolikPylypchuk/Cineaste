namespace Cineaste;

using System.Reflection;

using Avalonia.Controls.ApplicationLifetimes;

using FluentAvalonia.Styling;

using ReactiveUI.Validation.Formatters.Abstractions;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using Splat.Serilog;

using TransitioningContentControl = Avalonia.ReactiveUI.TransitioningContentControl;

public sealed class App : Application, IEnableLogger
{
    private readonly Mutex mutex;
    private readonly string appName;
    private readonly NamedPipeManager namedPipeManager;

    public App()
    {
        this.mutex = SingleInstanceManager.TryAcquireMutex();
        this.appName = Assembly.GetExecutingAssembly()?.GetName().Name ?? String.Empty;
        this.namedPipeManager = new NamedPipeManager(this.appName);
    }

    public override void Initialize() =>
        AvaloniaXamlLoader.Load(this);

    public override void RegisterServices()
    {
        BlobCache.ApplicationName = this.appName;
        base.RegisterServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += this.OnExit;
            RxApp.DefaultExceptionHandler = Observer.ToObserver<Exception>(this.OnException);
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            this.InitializeApp(desktop);

            this.Log().Info("Cineaste app started");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void InitializeApp(IClassicDesktopStyleApplicationLifetime desktop)
    {
        this.ConfigureLocator();
        this.ConfigureSuspensionDriver(desktop);
        this.ConfigureUI();

        var mainViewModel = new MainViewModel();

        desktop.MainWindow = this.CreateMainWindow(mainViewModel, desktop);
        desktop.MainWindow.Show();

        this.SetUpDialogs(new DialogHandler(desktop.MainWindow));

        this.namedPipeManager.StartServer();
        this.namedPipeManager.ReceivedString
            .Select(file => new OpenFileModel(file) { IsExternal = true })
            .InvokeCommand(mainViewModel.OpenFile);
    }

    private void ConfigureLocator()
    {
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

        Locator.CurrentMutable.RegisterConstant(RxApp.TaskpoolScheduler, TaskPoolKey);
        Locator.CurrentMutable.RegisterConstant(BlobCache.LocalMachine, CacheKey);
        Locator.CurrentMutable.RegisterConstant(BlobCache.UserAccount, StoreKey);

        Locator.CurrentMutable.RegisterConstant(Messages.ResourceManager);
        Locator.CurrentMutable.RegisterConstant(
            new LocalizedValidationTextFormatter(Messages.ResourceManager),
            typeof(IValidationTextFormatter<string>));

        this.RegisterBindingConverters();

        var preferences = BlobCache.UserAccount.GetObject<UserPreferences>(PreferencesKey)
            .Catch(Observable.FromAsync(this.CreateDefaultPreferencesAsync))
            .Wait();

        if (preferences == null)
        {
            string message = "Cannot get the preferences or create new ones";
            var exp = new InvalidOperationException(message);
            this.Log().Fatal(exp, message);
            throw exp;
        }

        Locator.CurrentMutable.RegisterConstant(preferences);

        var themeManager = new ThemeManager(preferences.UI.Theme);

        Locator.CurrentMutable.RegisterConstant(themeManager);
        Locator.CurrentMutable.RegisterConstant<IThemeAwareColorGenerator>(
            new ThemeAwareColorGenerator(themeManager));

        this.ConfigureLogging(preferences);
    }

    private void ConfigureSuspensionDriver(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var autoSuspendHelper = new AutoSuspendHelper(desktop);

        string file = Path.Combine(this.GetConfigDirectory(), AppStateFileName);

        RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new JsonSuspensionDriver<AppState>(file));

        autoSuspendHelper.OnFrameworkInitializationCompleted();
    }

    private void ConfigureUI()
    {
        TransitioningContentControl.PageTransitionProperty.OverrideDefaultValue(typeof(ViewModelViewHost), null);

        var themeManager = Locator.Current.GetService<ThemeManager>();
        var fluentAvaloniaTheme = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();

        themeManager.WhenAnyValue(tm => tm.Theme)
            .Select(Enum.GetName)
            .Subscribe(theme => fluentAvaloniaTheme!.RequestedTheme = theme);
    }

    private MainWindow CreateMainWindow(
        MainViewModel viewModel,
        IClassicDesktopStyleApplicationLifetime desktop)
    {
        var state = RxApp.SuspensionHost.GetAppState<AppState>();

        var window = new MainWindow
        {
            ViewModel = viewModel
        };

        if (state != null && state.IsInitialized)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Width = state.WindowWidth;
            window.Height = state.WindowHeight;
            window.Position = new((int)state.WindowX, (int)state.WindowY);
            window.WindowState = state.IsWindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        window.GetObservable(TopLevel.ClientSizeProperty)
            .Discard()
            .Merge(window.GetObservable(Window.WindowStateProperty)
                .Where(state => state != WindowState.Minimized)
                .Discard())
            .Merge(Observable.FromEventPattern<PixelPointEventArgs>(
                h => window.PositionChanged += h, h => window.PositionChanged -= h)
                .Discard())
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(() => this.SaveAppState(desktop));

        return window;
    }

    private void RegisterBindingConverters()
    {
        var seriesWatchStatusConverter = new SeriesWatchStatusConverter();
        var seriesReleaseStatusConverter = new SeriesReleaseStatusConverter();
        var seasonWatchStatusConverter = new SeasonWatchStatusConverter();
        var seasonReleaseStatusConverter = new SeasonReleaseStatusConverter();
        var filterTypeConverter = new FilterTypeConverter();
        var filterOperationConverter = new FilterOperationConverter();
        var listSortOrderConverter = new ListSortOrderConverter();
        var listSortDirectionConverter = new ListSortDirectionConverter();
        var logLevelConverter = new LogLevelConverter();

        this.RegisterBindingConverters(
            seriesWatchStatusConverter,
            seriesReleaseStatusConverter,
            seasonWatchStatusConverter,
            seasonReleaseStatusConverter,
            filterTypeConverter,
            filterOperationConverter,
            listSortOrderConverter,
            listSortDirectionConverter,
            logLevelConverter,
            new NumberConverter(),
            new ColorConverter(),
            new UriConverter());

        Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeriesWatchStatus>>(seriesWatchStatusConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeriesReleaseStatus>>(seriesReleaseStatusConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeasonWatchStatus>>(seasonWatchStatusConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeasonReleaseStatus>>(seasonReleaseStatusConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<FilterType>>(filterTypeConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<FilterOperation>>(filterOperationConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<ListSortOrder>>(listSortOrderConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<ListSortDirection>>(listSortDirectionConverter);
        Locator.CurrentMutable.RegisterConstant<IEnumConverter<LogEventLevel>>(logLevelConverter);
    }

    private void RegisterBindingConverters(params IBindingTypeConverter[] converters)
    {
        foreach (var converter in converters)
        {
            Locator.CurrentMutable.RegisterConstant(converter);
        }
    }

    private void ConfigureLogging(UserPreferences preferences)
    {
        var loggingLevelSwitch = new LoggingLevelSwitch((LogEventLevel)preferences.Logging.MinLogLevel);

        Locator.CurrentMutable.RegisterConstant(loggingLevelSwitch);

        Locator.CurrentMutable.UseSerilogFullLogger(new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(loggingLevelSwitch)
            .WriteTo.Debug(outputTemplate: LogTemplate)
            .WriteTo.Async(c => c.File(
                path: preferences.Logging.LogPath,
                outputTemplate: LogTemplate,
                fileSizeLimitBytes: 10000000,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 2))
            .Filter.ByIncludingOnly($"StartsWith(SourceContext, '{nameof(Cineaste)}')")
            .CreateLogger());
    }

    private async Task<UserPreferences> CreateDefaultPreferencesAsync()
    {
        var ui = new UIPreferences(Theme.Light);

        var file = new FilePreferences(showRecentFiles: true, new List<RecentFile>());

        var defaults = new DefaultsPreferences(
            Messages.DefaultDefaultSeasonTitle,
            Messages.DefaultDefaultSeasonOriginalTitle,
            this.CreateDefaultKinds(),
            new List<Tag>(),
            CultureInfo.GetCultureInfo("en-US"),
            ListSortOrder.ByTitle,
            ListSortOrder.ByYear,
            ListSortDirection.Ascending,
            ListSortDirection.Ascending);

        var logging = new LoggingPreferences(
            Path.Combine(this.GetConfigDirectory(), $"{this.appName}.log"),
            (int)LogEventLevel.Warning);

        var preferences = new UserPreferences(ui, file, defaults, logging);

        await BlobCache.UserAccount.InsertObject(PreferencesKey, preferences);

        return preferences;
    }

    private string GetConfigDirectory()
    {
        string root = PlatformDependent(
            windows: () => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            macos: GetUnixHomeFolder,
            linux: GetUnixHomeFolder);

        string directory = PlatformDependent(
            windows: () => this.appName, macos: () => $".{this.appName}", linux: () => $".{this.appName}");

        return Path.Combine(root, directory);
    }

    private List<Kind> CreateDefaultKinds()
    {
        const string black = "#FF000000";
        const string indigo = "#FF3949AB";
        const string green = "#FF43A047";
        const string blue = "#FF1E88E5";
        const string purple = "#FF5E35B1";

        const string red = "#FFE35953";
        const string darkRed = "#FFB71C1C";

        return new List<Kind>
            {
                new Kind
                {
                    Name = Messages.LiveAction,
                    ColorForWatchedMovie = black,
                    ColorForWatchedSeries = indigo,
                    ColorForNotWatchedMovie = red,
                    ColorForNotWatchedSeries = red,
                    ColorForNotReleasedMovie = darkRed,
                    ColorForNotReleasedSeries = darkRed
                },
                new Kind
                {
                    Name = Messages.Animation,
                    ColorForWatchedMovie = green,
                    ColorForWatchedSeries = blue,
                    ColorForNotWatchedMovie = red,
                    ColorForNotWatchedSeries = red,
                    ColorForNotReleasedMovie = darkRed,
                    ColorForNotReleasedSeries = darkRed
                },
                new Kind
                {
                    Name = Messages.Documentary,
                    ColorForWatchedMovie = purple,
                    ColorForWatchedSeries = purple,
                    ColorForNotWatchedMovie = red,
                    ColorForNotWatchedSeries = red,
                    ColorForNotReleasedMovie = darkRed,
                    ColorForNotReleasedSeries = darkRed
                }
            };
    }

    private void SetUpDialogs(DialogHandler handler)
    {
        Dialog.ShowMessage.RegisterHandler(handler.ShowMessageDialogAsync);
        Dialog.Confirm.RegisterHandler(handler.ShowConfirmDialogAsync);
        Dialog.Input.RegisterHandler(handler.ShowInputDialogAsync);
        Dialog.TagForm.RegisterHandler(handler.ShowTagFormDialogAsync);
        Dialog.SaveFile.RegisterHandler(handler.ShowSaveFileDialogAsync);
        Dialog.OpenFile.RegisterHandler(handler.ShowOpenFileDialogAsync);
        Dialog.ShowAbout.RegisterHandler(handler.ShowAboutDialogAsync);
    }

    private void SaveAppState(IClassicDesktopStyleApplicationLifetime desktop)
    {
        if (desktop.MainWindow == null)
        {
            return;
        }

        var state = RxApp.SuspensionHost.GetAppState<AppState>();

        state.IsWindowMaximized = desktop.MainWindow.WindowState == WindowState.Maximized;

        if (!state.IsWindowMaximized)
        {
            state.WindowWidth = desktop.MainWindow.Width;
            state.WindowHeight = desktop.MainWindow.Height;
            state.WindowX = desktop.MainWindow.Position.X;
            state.WindowY = desktop.MainWindow.Position.Y;
        }

        state.IsInitialized = true;
    }

    private void OnException(Notification<Exception> notification) =>
        this.Log().Error(notification.Value);

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e) =>
        this.CleanUp();

    private void CleanUp()
    {
        BlobCache.Shutdown().Wait();
        this.mutex.ReleaseMutex();
        this.mutex.Dispose();
    }
}
