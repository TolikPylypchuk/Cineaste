using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Akavache;

using MaterialDesignThemes.Wpf;

using MovieList.Data;
using MovieList.Infrastructure;
using MovieList.Preferences;
using MovieList.Properties;
using MovieList.ViewModels;

using ReactiveUI;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using Splat;
using Splat.Serilog;

using static MovieList.Constants;

namespace MovieList
{
    public partial class App : Application, IEnableLogger
    {
        private readonly Mutex mutex;
        private readonly NamedPipeManager namedPipeManager;

        public App()
        {
            this.mutex = SingleInstanceManager.TryAcquireMutex();
            this.namedPipeManager = new NamedPipeManager(Assembly.GetExecutingAssembly().FullName);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var mainViewModel = new MainViewModel();

            this.namedPipeManager.StartServer();
            this.namedPipeManager.ReceivedString.InvokeCommand(mainViewModel.OpenFile);

            BlobCache.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;

            await this.ConfigureLocatorAsync();

            mainViewModel.OpenFile.Subscribe(this.OnOpenFile);
            mainViewModel.CloseFile.Subscribe(this.OnCloseFile);

            this.MainWindow = this.CreateMainWindow(mainViewModel);
            this.MainWindow.Show();

            this.SetUpDialogs();

            this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.CleanUp();
            base.OnExit(e);
        }

        private async Task ConfigureLocatorAsync()
        {
            Locator.CurrentMutable.InitializeReactiveUI();
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

            Locator.CurrentMutable.RegisterConstant(BlobCache.LocalMachine, Cache);
            Locator.CurrentMutable.RegisterConstant(BlobCache.UserAccount, Store);

            var preferences = await BlobCache.UserAccount.GetObject<UserPreferences>(MainPreferences)
                .Catch(Observable.FromAsync(this.CreateDefaultPreferences));

            var loggingLevelSwitch = new LoggingLevelSwitch((LogEventLevel)preferences.Logging.MinLogLevel);

            Locator.CurrentMutable.RegisterConstant(preferences);
            Locator.CurrentMutable.RegisterConstant(loggingLevelSwitch);

            Locator.CurrentMutable.UseSerilogFullLogger(new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .WriteTo.Debug()
                .WriteTo.File(preferences.Logging.LogPath)
                .CreateLogger());
        }

        private async Task<UserPreferences> CreateDefaultPreferences()
        {
            var preferences = new UserPreferences(
                           new UIPreferences(0.0, 0.0, 0.0, 0.0, false, false),
                           new FilePreferences(true, new List<string>()),
                           new LoggingPreferences(
                               $"{Assembly.GetExecutingAssembly().GetName().Name}.log",
                               (int)LogEventLevel.Information));

            await BlobCache.UserAccount.InsertObject(MainPreferences, preferences);

            return preferences;
        }

        private MainWindow CreateMainWindow(MainViewModel viewModel)
        {
            var uiPreferences = Locator.Current.GetService<UserPreferences>().UI;

            var window = new MainWindow
            {
                ViewModel = viewModel
            };

            if (uiPreferences.IsInitialized)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Width = uiPreferences.WindowWidth;
                window.Height = uiPreferences.WindowHeight;
                window.Left = uiPreferences.WindowX;
                window.Top = uiPreferences.WindowY;
                window.WindowState = uiPreferences.IsWindowMaximized ? WindowState.Maximized : WindowState.Normal;
            }

            window.Events().SizeChanged
                .Merge(this.MainWindow.Events().StateChanged
                    .Where(_ => this.MainWindow.WindowState != WindowState.Minimized))
                .Merge(this.MainWindow.Events().LocationChanged)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Discard()
                .ObserveOnDispatcher()
                .SubscribeAsync(this.SaveUIPreferences);

            return window;
        }

        private void OnOpenFile(string file)
        {
            this.Log().Debug($"Opening a file: {file}");
            Locator.CurrentMutable.RegisterDatabaseServices(file);
        }

        private void OnCloseFile(string file)
        {
            this.Log().Debug($"Closing a file: {file}");
            Locator.CurrentMutable.UnregisterDatabaseServices(file);
        }

        private async Task SaveUIPreferences()
        {
            if (this.MainWindow == null)
            {
                return;
            }

            var preferences = Locator.Current.GetService<UserPreferences>();

            preferences.UI.WindowWidth = this.MainWindow.ActualWidth;
            preferences.UI.WindowHeight = this.MainWindow.ActualHeight;
            preferences.UI.WindowX = this.MainWindow.Left;
            preferences.UI.WindowY = this.MainWindow.Top;
            preferences.UI.IsWindowMaximized = this.MainWindow.WindowState == WindowState.Maximized;
            preferences.UI.IsInitialized = true;

            await BlobCache.UserAccount.InsertObject(MainPreferences, preferences);
        }

        private void SetUpDialogs()
        {
            Message.Show.RegisterHandler(async ctx =>
            {
                var viewModel = new MessageViewModel(ctx.Input, Messages.OK);
                var view = ViewLocator.Current.ResolveView(viewModel);
                view.ViewModel = viewModel;

                await DialogHost.Show(view);

                ctx.SetOutput(Unit.Default);
            });

            Message.Confirm.RegisterHandler(async ctx =>
            {
                var viewModel = new ConfirmationViewModel(ctx.Input, Messages.Confirm, Messages.Cancel);
                var view = ViewLocator.Current.ResolveView(viewModel);
                view.ViewModel = viewModel;

                var result = await DialogHost.Show(view);

                ctx.SetOutput(result is bool confirm && confirm);
            });
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
