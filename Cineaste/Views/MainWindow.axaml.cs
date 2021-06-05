using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.Models;
using Cineaste.Core.ViewModels;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Properties;

using DynamicData.Binding;

using ReactiveUI;

namespace Cineaste.Views
{
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.InitializeMainTabControl(disposables);
                this.InitializeMenu(disposables);

                this.ViewModel!.OpenFile
                    .WhereNotNull()
                    .Where(model => model.IsExternal)
                    .Discard()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(this.OnOpenFileExternally)
                    .DisposeWith(disposables);

                this.GetObservable(DragDrop.DropEvent)
                    .Where(e => e.Data.Contains(DataFormats.FileNames))
                    .SelectMany(e => e.Data.GetFileNames() ?? Enumerable.Empty<string>())
                    .Select(file => new OpenFileModel(file) { IsExternal = true })
                    .InvokeCommand(this.ViewModel.OpenFile)
                    .DisposeWith(disposables);

                Observable.FromEventPattern<CancelEventArgs>(h => this.Closing += h, h => this.Closing -= h)
                    .Do(e => e.EventArgs.Cancel = true)
                    .Discard()
                    .InvokeCommand(this.ViewModel.Shutdown)
                    .DisposeWith(disposables);

                this.ViewModel.Shutdown
                    .Subscribe(() =>
                    {
                        disposables.Dispose();
                        this.Close();
                    })
                    .DisposeWith(disposables);
            });
        }

        private void InitializeMainTabControl(CompositeDisposable disposables)
        {
            this.AddTab(new TabItem
            {
                Header = Messages.HomePage,
                Content = new ViewModelViewHost { ViewModel = this.ViewModel!.HomePage }
            });

            this.ViewModel!.Files
                .ToObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ActOnEveryObject(this.OnFileAdded, this.OnFileRemoved)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Preferences)
                .ObserveOn(RxApp.MainThreadScheduler)
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
                .DisposeWith(disposables);
        }

        private void OnFileAdded(FileViewModel vm)
        {
            var tabItem = new TabItem
            {
                Header = new ViewModelViewHost { ViewModel = vm.Header },
                Content = new ViewModelViewHost { ViewModel = vm },
                Tag = vm.FileName
            };

            if (this.ViewModel?.Preferences == null)
            {
                this.AddTab(tabItem);
            } else
            {
                this.InsertTab(^1, tabItem);
            }
        }

        private void OnFileRemoved(FileViewModel vm) =>
            this.RemoveTab(this.MainTabControl.Items
                .OfType<TabItem>()
                .First(item => vm.FileName.Equals(item.Tag)));

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
                .DisposeWith(disposables);

            this.BindKeyToCommand(Key.N, KeyModifiers.Control, this.ViewModel!.HomePage.CreateFile)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.HomePage.OpenFile, v => v.OpenMenuItem)
                .DisposeWith(disposables);

            this.GetObservable(KeyUpEvent)
                .Where(e => e.Key == Key.O && this.IsDown(e.KeyModifiers, KeyModifiers.Control))
                .Select(e => (string?)null)
                .InvokeCommand(this.ViewModel.HomePage.OpenFile)
                .DisposeWith(disposables);

            this.ViewModel.HomePage.RecentFiles
                .ActOnEveryObject(this.OnRecentFileAdded, this.OnRecentFileRemoved)
                .DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.HomePage.RecentFiles.Count,
                    v => v.OpenRecentMenuItem.IsEnabled,
                    count => count > 0)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Shutdown, v => v.ExitMenuItem)
                .DisposeWith(disposables);
        }

        private void OnRecentFileAdded(RecentFileViewModel file)
        {
            int index = this.ViewModel!.HomePage.RecentFiles.IndexOf(file);

            var item = new MenuItem
            {
                Header = file.File.Path,
                Command = this.ViewModel.OpenFile,
                CommandParameter = new OpenFileModel(file.File.Path),
                Tag = file.File.Path
            };

            var items = this.OpenRecentMenuItem.Items.OfType<MenuItem>().ToList();
            items.Insert(index, item);

            this.OpenRecentMenuItem.Items = items;
        }

        private void OnRecentFileRemoved(RecentFileViewModel file)
        {
            var item = this.OpenRecentMenuItem.Items
                .OfType<MenuItem>()
                .First(item => file.File.Path.Equals(item.Tag));

            this.OpenRecentMenuItem.Items = this.OpenRecentMenuItem.Items
                .OfType<MenuItem>()
                .Where(menuItem => menuItem != item)
                .ToList();
        }

        private void InitializeFileMenu(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveMenuItem)
                .DisposeWith(disposables);

            this.BindKeyToCommand(Key.S, KeyModifiers.Control, this.ViewModel!.Save)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SaveAs, v => v.SaveAsMenuItem)
                .DisposeWith(disposables);

            this.BindKeyToCommand(Key.S, KeyModifiers.Control | KeyModifiers.Shift, this.ViewModel.SaveAs)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.OpenSettings, v => v.SettingsMenuItem)
                .DisposeWith(disposables);

            this.BindKeyToCommand(Key.P, KeyModifiers.Control, this.ViewModel.OpenSettings)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.CloseCurrentTab, v => v.CloseMenuItem)
                .DisposeWith(disposables);

            this.BindKeyToCommand(Key.W, KeyModifiers.Control, this.ViewModel.CloseCurrentTab)
                .DisposeWith(disposables);
        }

        private void InitializeEditMenu(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.OpenPreferences, v => v.PreferencesMenuItem)
                .DisposeWith(disposables);

            this.BindKeyToCommand(Key.P, KeyModifiers.Control | KeyModifiers.Shift, this.ViewModel!.OpenPreferences)
                .DisposeWith(disposables);
        }

        private void InitializeHelpMenu(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.ShowAbout, v => v.AboutMenuItem)
                .DisposeWith(disposables);

            this.BindKeyToCommand(Key.F1, this.ViewModel!.ShowAbout)
                .DisposeWith(disposables);
        }

        private IDisposable BindKeyToCommand<T>(Key key, ReactiveCommand<Unit, T> command) =>
            this.GetObservable(KeyUpEvent)
                .Where(e => e.Key == key)
                .Discard()
                .InvokeCommand(command);

        private IDisposable BindKeyToCommand<T>(Key key, KeyModifiers modifiers, ReactiveCommand<Unit, T> command) =>
            this.GetObservable(KeyUpEvent)
                .Where(e => e.Key == key && this.IsDown(e.KeyModifiers, modifiers))
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

        private void OnOpenPreferences(PreferencesFormViewModel vm) =>
            this.AddTab(new TabItem
            {
                Header = new ViewModelViewHost { ViewModel = vm.Header },
                Content = new ViewModelViewHost { ViewModel = vm },
                Tag = String.Empty
            });

        private void OnClosePreferences()
        {
            var preferencesTab = this.MainTabControl.Items
                .OfType<TabItem>()
                .Where(item => item.Tag != null)
                .FirstOrDefault(item => String.IsNullOrEmpty(item.Tag?.ToString()));

            if (preferencesTab != null)
            {
                this.RemoveTab(preferencesTab);
            }
        }

        private bool IsDown(KeyModifiers modifiersToCheck, params KeyModifiers[] modifiers) =>
            modifiersToCheck == modifiers.Aggregate((acc, key) => acc | key);

        private void AddTab(TabItem item) =>
            this.MainTabControl.Items = this.MainTabControl.Items.OfType<TabItem>().Append(item).ToList();

        private void InsertTab(Index index, TabItem item)
        {
            var items = this.MainTabControl.Items.OfType<TabItem>().ToList();
            items.Insert(index.IsFromEnd ? items.Count - index.Value : index.Value, item);
            this.MainTabControl.Items = items;
        }

        private void RemoveTab(TabItem item) =>
            this.MainTabControl.Items = this.MainTabControl.Items.OfType<TabItem>().Where(tab => tab != item).ToList();
    }
}
