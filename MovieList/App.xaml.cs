using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Akavache;

using MovieList.Data;
using MovieList.Infrastructure;
using MovieList.Preferences;
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

            this.MainWindow = new MainWindow
            {
                ViewModel = mainViewModel
            };

            this.MainWindow.Show();

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
                           new UIPreferences(-1, -1, -1, -1, false),
                           new FilePreferences(true, new List<string>()),
                           new LoggingPreferences(
                               $"{Assembly.GetExecutingAssembly().GetName().Name}.log",
                               (int)LogEventLevel.Information));

            await BlobCache.UserAccount.InsertObject(MainPreferences, preferences);

            return preferences;
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

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
            => this.CleanUp();

        private void CleanUp()
        {
            BlobCache.Shutdown().Wait();
            this.mutex.ReleaseMutex();
            this.mutex.Dispose();
        }
    }
}
