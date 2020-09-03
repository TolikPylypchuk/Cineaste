using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DynamicData.Binding;

using MovieList.Core;
using MovieList.Core.Models;
using MovieList.Core.ViewModels;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList
{
    public abstract class MainWindowBase : ReactiveWindow<MainViewModel> { }

    public partial class MainWindow : MainWindowBase
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.InitializeMainTabControl(disposables);
                this.InitializeMenu(disposables);

                this.ViewModel!.OpenFile
                    .WhereNotNull()
                    .Where(model => model.IsExternal)
                    .Discard()
                    .ObserveOnDispatcher()
                    .Subscribe(this.OnOpenFileExternally)
                    ?.DisposeWith(disposables);

                this.Events().Drop
                    .Where(e => e.Data.GetDataPresent(DataFormats.FileDrop))
                    .SelectMany(e => (string[])e.Data.GetData(DataFormats.FileDrop))
                    .Select(file => new OpenFileModel(file, true))
                    .InvokeCommand(this.ViewModel.OpenFile)
                    ?.DisposeWith(disposables);

                this.Events().Closing
                    .Do(e => e.Cancel = true)
                    .Discard()
                    .InvokeCommand(this.ViewModel.Shutdown)
                    ?.DisposeWith(disposables);

                this.ViewModel.Shutdown
                    .Do(unit => disposables.Dispose())
                    .Subscribe(this.Close)
                    ?.DisposeWith(disposables);
            });
        }

        private void InitializeMainTabControl(CompositeDisposable disposables)
        {
            this.MainTabControl.Items.Add(new TabItem
            {
                Header = Messages.HomePage,
                Content = new ViewModelViewHost { ViewModel = this.ViewModel!.HomePage }
            });

            this.ViewModel!.Files
                .ToObservableChangeSet()
                .ObserveOnDispatcher()
                .ActOnEveryObject(
                    vm => this.MainTabControl.Items.Add(new TabItem
                    {
                        Header = new ViewModelViewHost { ViewModel = vm.Header },
                        Content = new ViewModelViewHost { ViewModel = vm },
                        Tag = vm.FileName
                    }),
                    vm => this.MainTabControl.Items.Remove(this.MainTabControl.Items
                        .Cast<TabItem>()
                        .First(item => vm.FileName.Equals(item.Tag))))
                ?.DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Preferences)
                .ObserveOnDispatcher()
                .Subscribe(vm =>
                {
                    if (vm != null)
                    {
                        this.OnOpenPreferences(vm);
                    } else
                    {
                        this.OnClosePreferences();
                    }
                });

            this.Bind(this.ViewModel, vm => vm.SelectedItemIndex, v => v.MainTabControl.SelectedIndex)
                ?.DisposeWith(disposables);
        }

        private void InitializeMenu(CompositeDisposable disposables)
        {
            this.InitializeMainMenu(disposables);
            this.InitializeFileMenu(disposables);
            this.InitializeEditMenu(disposables);
            this.InitializeHelpMenu(disposables);
        }

        private void InitializeMainMenu(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.HomePage.CreateFile, v => v.NewMenuItem)
                ?.DisposeWith(disposables);

            this.BindKeyToCommand(Key.N, ModifierKeys.Control, this.ViewModel!.HomePage.CreateFile)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.HomePage.OpenFile, v => v.OpenMenuItem)
                ?.DisposeWith(disposables);

            this.Events().KeyUp
                .Where(e => e.Key == Key.O && this.IsDown(ModifierKeys.Control))
                .Select(e => (string?)null)
                .InvokeCommand(this.ViewModel.HomePage.OpenFile)
                ?.DisposeWith(disposables);

            this.ViewModel.HomePage.RecentFiles
                .ActOnEveryObject(
                    file => this.OpenRecentMenuItem.Items.Insert(
                        this.ViewModel.HomePage.RecentFiles.IndexOf(file),
                        new MenuItem
                        {
                            Header = file.File.Path,
                            Command = this.ViewModel.OpenFile,
                            CommandParameter = new OpenFileModel(file.File.Path),
                            Tag = file.File.Path
                        }),
                    file => this.OpenRecentMenuItem.Items.Remove(this.OpenRecentMenuItem.Items
                        .Cast<MenuItem>()
                        .First(item => file.File.Path.Equals(item.Tag))))
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.HomePage.RecentFiles.Count,
                    v => v.OpenRecentMenuItem.IsEnabled,
                    count => count > 0)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Shutdown, v => v.ExitMenuItem)
                ?.DisposeWith(disposables);
        }

        private void InitializeFileMenu(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveMenuItem)
                ?.DisposeWith(disposables);

            this.BindKeyToCommand(Key.S, ModifierKeys.Control, this.ViewModel!.Save)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SaveAs, v => v.SaveAsMenuItem)
                ?.DisposeWith(disposables);

            this.BindKeyToCommand(Key.S, ModifierKeys.Control | ModifierKeys.Shift, this.ViewModel.SaveAs)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.OpenSettings, v => v.SettingsMenuItem)
                ?.DisposeWith(disposables);

            this.BindKeyToCommand(Key.P, ModifierKeys.Control, this.ViewModel.OpenSettings)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.CloseCurrentTab, v => v.CloseMenuItem)
                ?.DisposeWith(disposables);

            this.BindKeyToCommand(Key.W, ModifierKeys.Control, this.ViewModel.CloseCurrentTab)
                ?.DisposeWith(disposables);
        }

        private void InitializeEditMenu(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.OpenPreferences, v => v.PreferencesMenuItem)
                ?.DisposeWith(disposables);

            this.BindKeyToCommand(Key.P, ModifierKeys.Control | ModifierKeys.Shift, this.ViewModel!.OpenPreferences)
                ?.DisposeWith(disposables);
        }

        private void InitializeHelpMenu(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.ShowAbout, v => v.AboutMenuItem)
                ?.DisposeWith(disposables);

            this.BindKeyToCommand(Key.F1, this.ViewModel!.ShowAbout)
                ?.DisposeWith(disposables);
        }

        private IDisposable BindKeyToCommand<T>(Key key, ReactiveCommand<Unit, T> command)
            => this.Events().KeyUp
                .Where(e => e.Key == key)
                .Discard()
                .InvokeCommand(command);

        private IDisposable BindKeyToCommand<T>(Key key, ModifierKeys modifierKeys, ReactiveCommand<Unit, T> command)
            => this.Events().KeyUp
                .Where(e => e.Key == key && this.IsDown(modifierKeys))
                .Discard()
                .InvokeCommand(command);

        private void OnOpenFileExternally()
        {
            if (!this.IsVisible)
            {
                this.Show();
            }

            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }

            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        private void OnOpenPreferences(PreferencesFormViewModel vm)
            => this.MainTabControl.Items.Add(new TabItem
            {
                Header = new ViewModelViewHost { ViewModel = vm.Header },
                Content = new ViewModelViewHost { ViewModel = vm },
                Tag = String.Empty
            });

        private void OnClosePreferences()
        {
            var preferencesTab = this.MainTabControl.Items
                .Cast<TabItem>()
                .Where(item => item.Tag != null)
                .FirstOrDefault(item => String.IsNullOrEmpty(item.Tag.ToString()));

            if (preferencesTab != null)
            {
                this.MainTabControl.Items.Remove(preferencesTab);
            }
        }

        private bool IsDown(params ModifierKeys[] keys)
            => Keyboard.Modifiers == keys.Aggregate((acc, key) => acc | key);
    }
}
