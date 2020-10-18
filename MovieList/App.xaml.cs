using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Akavache;

using MovieList.Converters;
using MovieList.Core;
using MovieList.Core.Models;
using MovieList.Core.Preferences;
using MovieList.Core.State;
using MovieList.Core.ViewModels;
using MovieList.Core.ViewModels.Filters;
using MovieList.Data.Models;
using MovieList.Infrastructure;
using MovieList.Properties;

using ReactiveUI;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using Splat;
using Splat.Serilog;

using static MovieList.Core.Constants;

namespace MovieList
{
    public partial class App : Application, IEnableLogger
    {
        private readonly Mutex mutex;
        private readonly NamedPipeManager namedPipeManager;

        public App()
        {
            this.mutex = SingleInstanceManager.TryAcquireMutex();
            this.namedPipeManager = new NamedPipeManager(Assembly.GetExecutingAssembly()?.FullName ?? String.Empty);

            var autoSuspendHelper = new AutoSuspendHelper(this);
            GC.KeepAlive(autoSuspendHelper);

            BlobCache.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await this.ConfigureLocatorAsync();

            RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
            RxApp.SuspensionHost.SetupDefaultSuspendResume();

            base.OnStartup(e);

            var mainViewModel = new MainViewModel();

            var mainWindow = this.CreateMainWindow(mainViewModel);

            this.MainWindow = mainWindow;
            this.MainWindow.Show();

            this.namedPipeManager.StartServer();
            this.namedPipeManager.ReceivedString
                .Select(file => new OpenFileModel(file) { IsExternal = true })
                .InvokeCommand(mainViewModel.OpenFile);

            this.SetUpDialogs(new DialogHandler(mainWindow.MainDialogHost));

            this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;

            this.Log().Info("MovieList app started");

            if (e.Args.Length > 0)
            {
                await mainViewModel.OpenFile.Execute(new OpenFileModel(e.Args[0]));
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.CleanUp();
        }

        private async Task ConfigureLocatorAsync()
        {
            Locator.CurrentMutable.InitializeReactiveUI();
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
            Locator.CurrentMutable.RegisterSuspensionDriver();

            Locator.CurrentMutable.Register<ICreatesObservableForProperty>(() => new CustomPropertyResolver());

            Locator.CurrentMutable.RegisterConstant(RxApp.TaskpoolScheduler, TaskPoolKey);
            Locator.CurrentMutable.RegisterConstant(BlobCache.LocalMachine, CacheKey);
            Locator.CurrentMutable.RegisterConstant(BlobCache.UserAccount, StoreKey);

            Locator.CurrentMutable.RegisterConstant(Messages.ResourceManager);

            this.RegisterBindingConverters();

            var preferences = await BlobCache.UserAccount.GetObject<UserPreferences>(PreferencesKey)
                .Catch(Observable.FromAsync(this.CreateDefaultPreferencesAsync));

            var loggingLevelSwitch = new LoggingLevelSwitch((LogEventLevel)preferences.Logging.MinLogLevel);

            Locator.CurrentMutable.RegisterConstant(preferences);
            Locator.CurrentMutable.RegisterConstant(loggingLevelSwitch);

            Locator.CurrentMutable.UseSerilogFullLogger(new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .WriteTo.Debug()
                .WriteTo.File(preferences.Logging.LogPath)
                .CreateLogger());
        }

        private void RegisterBindingConverters()
        {
            var seriesWatchStatusConverter = new SeriesWatchStatusConverter();
            var seriesReleaseStatusConverter = new SeriesReleaseStatusConverter();
            var seasonWatchStatusConverter = new SeasonWatchStatusConverter();
            var seasonReleaseStatusConverter = new SeasonReleaseStatusConverter();
            var filterTypeConverter = new FilterTypeConverter();
            var filterOperationConverter = new FilterOperationConverter();

            this.RegisterBindingConverters(
                seriesWatchStatusConverter,
                seriesReleaseStatusConverter,
                seasonWatchStatusConverter,
                seasonReleaseStatusConverter,
                filterTypeConverter,
                filterOperationConverter,
                new BrushToHexConverter(),
                new ColorToBrushConverter(),
                new BooleanToVisibilityTypeConverter());

            Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeriesWatchStatus>>(seriesWatchStatusConverter);
            Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeriesReleaseStatus>>(seriesReleaseStatusConverter);
            Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeasonWatchStatus>>(seasonWatchStatusConverter);
            Locator.CurrentMutable.RegisterConstant<IEnumConverter<SeasonReleaseStatus>>(seasonReleaseStatusConverter);
            Locator.CurrentMutable.RegisterConstant<IEnumConverter<FilterType>>(filterTypeConverter);
            Locator.CurrentMutable.RegisterConstant<IEnumConverter<FilterOperation>>(filterOperationConverter);
        }

        private void RegisterBindingConverters(params IBindingTypeConverter[] converters)
        {
            foreach (var converter in converters)
            {
                Locator.CurrentMutable.RegisterConstant(converter);
            }
        }

        private async Task<UserPreferences> CreateDefaultPreferencesAsync()
        {
            var filePreferences = new FilePreferences(showRecentFiles: true, new List<RecentFile>());

            var defaultsPreferences = new DefaultsPreferences(
                Messages.DefaultDefaultSeasonTitle,
                Messages.DefaultDefaultSeasonOriginalTitle,
                this.CreateDefaultKinds(),
                new List<Tag>(),
                CultureInfo.GetCultureInfo("uk-UA"));

            string appName = Assembly.GetExecutingAssembly()?.GetName().Name ?? String.Empty;

            var loggingPreferences = new LoggingPreferences(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    appName,
                    $"{appName}.log"),
                (int)LogEventLevel.Warning);

            var preferences = new UserPreferences(filePreferences, defaultsPreferences, loggingPreferences);

            await BlobCache.UserAccount.InsertObject(PreferencesKey, preferences);

            return preferences;
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

        private MainWindow CreateMainWindow(MainViewModel viewModel)
        {
            var state = RxApp.SuspensionHost.GetAppState<AppState>();

            var window = new MainWindow
            {
                ViewModel = viewModel
            };

            if (state.IsInitialized)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Width = state.WindowWidth;
                window.Height = state.WindowHeight;
                window.Left = state.WindowX;
                window.Top = state.WindowY;
                window.WindowState = state.IsWindowMaximized ? WindowState.Maximized : WindowState.Normal;
            }

            window.Events().SizeChanged
                .Merge(window.Events().StateChanged
                    .Where(e => window.WindowState != WindowState.Minimized))
                .Merge(window.Events().LocationChanged)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Discard()
                .ObserveOnDispatcher()
                .Subscribe(this.SaveAppState);

            return window;
        }

        private void SaveAppState()
        {
            if (this.MainWindow == null)
            {
                return;
            }

            var state = RxApp.SuspensionHost.GetAppState<AppState>();

            state.WindowWidth = this.MainWindow.ActualWidth;
            state.WindowHeight = this.MainWindow.ActualHeight;
            state.WindowX = this.MainWindow.Left;
            state.WindowY = this.MainWindow.Top;
            state.IsWindowMaximized = this.MainWindow.WindowState == WindowState.Maximized;
            state.IsInitialized = true;
        }

        private void SetUpDialogs(DialogHandler handler)
        {
            Dialog.ShowMessage.RegisterHandler(handler.ShowMessageDialogAsync);
            Dialog.Confirm.RegisterHandler(handler.ShowConfirmDialogAsync);
            Dialog.Input.RegisterHandler(handler.ShowInputDialogAsync);
            Dialog.ColorPicker.RegisterHandler(handler.ShowColorDialogAsync);
            Dialog.TagForm.RegisterHandler(handler.ShowTagFormDialogAsync);
            Dialog.SaveFile.RegisterHandler(handler.ShowSaveFileDialogAsync);
            Dialog.OpenFile.RegisterHandler(handler.ShowOpenFileDialogAsync);
            Dialog.ShowAbout.RegisterHandler(handler.ShowAboutDialogAsync);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Log().Fatal(e.Exception);
            this.CleanUp();
        }

        private void CleanUp()
        {
            BlobCache.Shutdown().Wait();
            this.mutex.ReleaseMutex();
            this.mutex.Dispose();
        }
    }
}
