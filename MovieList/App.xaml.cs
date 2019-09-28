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
using Serilog.Events;

using Splat;
using Splat.Serilog;

namespace MovieList
{
    public partial class App : Application
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
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
            Locator.CurrentMutable.RegisterDatabaseServices();

            Locator.CurrentMutable.UseSerilogFullLogger(new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch { MinimumLevel = LogEventLevel.Information })
                .WriteTo.Debug()
                .WriteTo.File("MovieList.log")
                .CreateLogger());
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
