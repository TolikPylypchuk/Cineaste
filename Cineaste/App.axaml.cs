using System;
using System.Reflection;
using System.Threading;

using Akavache;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.State;
using Cineaste.Infrastructure;
using Cineaste.Views;

using ReactiveUI;

using Splat;

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
            PlatformRegistrationManager.SetRegistrationNamespaces(RegistrationNamespace.Avalonia);
            BlobCache.ApplicationName = Assembly.GetExecutingAssembly().GetName()?.Name ?? String.Empty;

            base.RegisterServices();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += this.OnExit;
                this.InitializeApp(desktop);
                this.Log().Info("Cineaste app started");
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void InitializeApp(IClassicDesktopStyleApplicationLifetime desktop)
        {
            this.ConfigureLocator();

            var suspension = new AutoSuspendHelper(desktop);
            RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
            RxApp.SuspensionHost.SetupDefaultSuspendResume();
            suspension.OnFrameworkInitializationCompleted();

            desktop.MainWindow = new MainWindow();

            this.namedPipeManager.StartServer();
        }

        private void ConfigureLocator()
        {
            Locator.CurrentMutable.RegisterSuspensionDriver();
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            BlobCache.Shutdown().Wait();
            this.mutex.ReleaseMutex();
            this.mutex.Dispose();
        }
    }
}
