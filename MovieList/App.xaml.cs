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

using MaterialDesignExtensions.Controls;

using MaterialDesignThemes.Wpf;
using MovieList.Data;
using MovieList.DialogModels;
using MovieList.Infrastructure;
using MovieList.Preferences;
using MovieList.Properties;
using MovieList.State;
using MovieList.ViewModels;

using ReactiveUI;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using Splat;
using Splat.Serilog;

using static MovieList.Constants;
using static MovieList.Data.Constants;

namespace MovieList
{
    public partial class App : Application, IEnableLogger
    {
        private readonly Mutex mutex;
        private readonly NamedPipeManager namedPipeManager;
        private DialogHost mainDialogHost = null!;

        public App()
        {
            this.mutex = SingleInstanceManager.TryAcquireMutex();
            this.namedPipeManager = new NamedPipeManager(Assembly.GetExecutingAssembly().FullName);

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

            this.MainWindow = this.CreateMainWindow(mainViewModel);
            this.MainWindow.Show();

            this.namedPipeManager.StartServer();
            this.namedPipeManager.ReceivedString
                .Select(file => new OpenFileModel(file, true))
                .InvokeCommand(mainViewModel.OpenFile);

            this.SetUpDialogs();

            this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;

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

            Locator.CurrentMutable.Register(() => new CustomPropertyResolver(), typeof(ICreatesObservableForProperty));

            Locator.CurrentMutable.RegisterConstant(BlobCache.LocalMachine, CacheKey);
            Locator.CurrentMutable.RegisterConstant(BlobCache.UserAccount, StoreKey);

            Locator.CurrentMutable.Register(
                () => new Settings(
                    String.Empty,
                    ListFileVersion,
                    Messages.DefaultSeasonTitle,
                    Messages.DefaultSeasonOriginalTitle),
                NewSettingsKey);

            var preferences = await BlobCache.UserAccount.GetObject<UserPreferences>(PreferencesKey)
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
                new FilePreferences(true, new List<RecentFile>()),
                new LoggingPreferences(
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.log",
                    (int)LogEventLevel.Information));

            await BlobCache.UserAccount.InsertObject(PreferencesKey, preferences);

            return preferences;
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
                .Merge(this.MainWindow.Events().StateChanged
                    .Where(_ => this.MainWindow.WindowState != WindowState.Minimized))
                .Merge(this.MainWindow.Events().LocationChanged)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Discard()
                .ObserveOnDispatcher()
                .Subscribe(this.SaveAppState);

            this.mainDialogHost = window.MainDialogHost;

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

        private void SetUpDialogs()
        {
            this.SetUpShowDialog();
            this.SetUpConfirmDialog();
            this.SetUpCreateListDialog();
            this.SetUpSaveFileDialog();
            this.SetUpOpenFileDialog();
        }

        private void SetUpShowDialog()
            => Dialog.Show.RegisterHandler(async ctx =>
            {
                var viewModel = new MessageModel(
                    Messages.ResourceManager.GetString(ctx.Input) ?? String.Empty, Messages.OK);

                var view = ViewLocator.Current.ResolveView(viewModel);
                view.ViewModel = viewModel;

                await DialogHost.Show(view);

                ctx.SetOutput(Unit.Default);
            });

        private void SetUpConfirmDialog()
            => Dialog.Confirm.RegisterHandler(async ctx =>
            {
                var viewModel = new ConfirmationModel(
                    Messages.ResourceManager.GetString(ctx.Input) ?? String.Empty, Messages.Confirm, Messages.Cancel);

                var view = ViewLocator.Current.ResolveView(viewModel);
                view.ViewModel = viewModel;

                var result = await DialogHost.Show(view);

                ctx.SetOutput(result is bool confirm && confirm);
            });

        public void SetUpCreateListDialog()
            => Dialog.Input.RegisterHandler(async ctx =>
            {
                var viewModel = new InputModel(
                    Messages.ResourceManager.GetString(ctx.Input) ?? String.Empty, Messages.Confirm, Messages.Cancel);

                var view = ViewLocator.Current.ResolveView(viewModel);
                view.ViewModel = viewModel;

                var result = await DialogHost.Show(view);

                ctx.SetOutput(result is string value ? value : null);
            });

        public void SetUpSaveFileDialog()
            => Dialog.SaveFile.RegisterHandler(async ctx =>
            {
                var dialogArgs = new SaveFileDialogArguments
                {
                    Width = 1000,
                    Height = 600,
                    Filters = $"{Messages.FileExtensionDescription}|*.{ListFileExtension}|" +
                              $"{Messages.AllExtensionsDescription}|*",
                    CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Filename = ctx.Input,
                    ForceFileExtensionOfFileFilter = true,
                    CreateNewDirectoryEnabled = true
                };

                var result = await SaveFileDialog.ShowDialogAsync(this.mainDialogHost, dialogArgs);

                ctx.SetOutput(result == null || result.Canceled ? null : result.File);
            });

        private void SetUpOpenFileDialog()
            => Dialog.OpenFile.RegisterHandler(async ctx =>
            {
                var dialogArgs = new OpenFileDialogArguments
                {
                    Width = 1000,
                    Height = 600,
                    Filters = $"{Messages.FileExtensionDescription}|*.{ListFileExtension}|" +
                              $"{Messages.AllExtensionsDescription}|*",
                    CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                var result = await OpenFileDialog.ShowDialogAsync(this.mainDialogHost, dialogArgs);

                ctx.SetOutput(result == null || result.Canceled ? null : result.File);
            });

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
