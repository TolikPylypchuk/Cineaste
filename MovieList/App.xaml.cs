using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using MovieList.Data;
using MovieList.Infrastructure;
using MovieList.ViewModels;

using ReactiveUI;

using Serilog;
using Serilog.Core;

using Splat;
using Splat.Serilog;

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

        protected override void OnStartup(StartupEventArgs e)
        {
            var viewModel = new MainViewModel();

            this.namedPipeManager.StartServer();
            this.namedPipeManager.ReceivedString.InvokeCommand(viewModel.OpenFile);

            this.ConfigureLocator();

            viewModel.OpenFile.Subscribe(this.OnOpenFile);
            viewModel.CloseFile.Subscribe(this.OnCloseFile);

            this.MainWindow = new MainWindow
            {
                ViewModel = viewModel
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

        private void ConfigureLocator()
        {
            Locator.CurrentMutable.InitializeReactiveUI();
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

            Locator.CurrentMutable.UseSerilogFullLogger(new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch())
                .WriteTo.Debug()
                .WriteTo.File("MovieList.log")
                .CreateLogger());
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
            this.mutex.ReleaseMutex();
            this.mutex.Dispose();
        }
    }
}
