using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Akavache;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.Models;
using Cineaste.Core.Preferences;
using Cineaste.Core.State;
using Cineaste.Core.ViewModels;
using Cineaste.Data;
using Cineaste.Data.Models;
using Cineaste.Infrastructure;
using Cineaste.Properties;
using Cineaste.Views;

using ReactiveUI;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using Splat;
using Splat.Serilog;

using static Cineaste.Constants;
using static Cineaste.Core.Constants;
using static Cineaste.Core.Util;

namespace Cineaste
{
    public sealed class App : Application, IEnableLogger
    {
        private readonly Mutex mutex;
        private readonly NamedPipeManager namedPipeManager;

        public App()
        {
            this.mutex = SingleInstanceManager.TryAcquireMutex();
            this.namedPipeManager = new NamedPipeManager(Assembly.GetExecutingAssembly()?.FullName ?? String.Empty);
        }

        public override void Initialize() =>
            AvaloniaXamlLoader.Load(this);

        public override void RegisterServices()
        {
            BlobCache.ApplicationName = Assembly.GetExecutingAssembly().GetName()?.Name ?? String.Empty;
            base.RegisterServices();
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += this.OnExit;
                await this.InitializeApp(desktop);
                this.Log().Info("Cineaste app started");
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async Task InitializeApp(IClassicDesktopStyleApplicationLifetime desktop)
        {
            await this.ConfigureLocator();
            this.ConfigureSuspensionDriver(desktop);

            var mainViewModel = new MainViewModel();

            desktop.MainWindow = this.CreateMainWindow(mainViewModel, desktop);
            desktop.MainWindow.Show();

            this.SetUpDialogs(new DialogHandler(desktop.MainWindow));

            this.namedPipeManager.StartServer();
            this.namedPipeManager.ReceivedString
                .Select(file => new OpenFileModel(file) { IsExternal = true })
                .InvokeCommand(mainViewModel.OpenFile);
        }

        private async Task ConfigureLocator()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

            Locator.CurrentMutable.RegisterConstant(RxApp.TaskpoolScheduler, TaskPoolKey);
            Locator.CurrentMutable.RegisterConstant(BlobCache.LocalMachine, CacheKey);
            Locator.CurrentMutable.RegisterConstant(BlobCache.UserAccount, StoreKey);

            Locator.CurrentMutable.RegisterConstant(Messages.ResourceManager);

            this.RegisterBindingConverters();

            var preferences = await BlobCache.UserAccount.GetObject<UserPreferences>(PreferencesKey)
                .Catch(Observable.FromAsync(this.CreateDefaultPreferencesAsync));

            Locator.CurrentMutable.RegisterConstant(preferences);

            this.ConfigureLogging(preferences);
        }

        private void ConfigureSuspensionDriver(IClassicDesktopStyleApplicationLifetime desktop)
        {
            var autoSuspendHelper = new AutoSuspendHelper(desktop);

            static string GetUnixHomeFolder() =>
                Environment.GetEnvironmentVariable("HOME") ?? String.Empty;

            string root = PlatformDependent(
                    windows: () => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    macos: GetUnixHomeFolder,
                    linux: GetUnixHomeFolder);

            string folder = Assembly.GetExecutingAssembly().GetName().Name ?? String.Empty;

            string file = Path.Combine(root, folder, AppStateFileName);

            RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(new JsonSuspensionDriver<AppState>(file));

            autoSuspendHelper.OnFrameworkInitializationCompleted();
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
        { }

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
            var filePreferences = new FilePreferences(showRecentFiles: true, new List<RecentFile>());

            var defaultsPreferences = new DefaultsPreferences(
                Messages.DefaultDefaultSeasonTitle,
                Messages.DefaultDefaultSeasonOriginalTitle,
                this.CreateDefaultKinds(),
                new List<Tag>(),
                CultureInfo.GetCultureInfo("en-US"),
                ListSortOrder.ByTitle,
                ListSortOrder.ByYear,
                ListSortDirection.Ascending,
                ListSortDirection.Ascending);

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

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            BlobCache.Shutdown().Wait();
            this.mutex.ReleaseMutex();
            this.mutex.Dispose();
        }
    }
}
