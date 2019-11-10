using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

using DynamicData;
using DynamicData.Binding;

using MovieList.Properties;
using MovieList.ViewModels;

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
                    .DisposeWith(disposables);

                this.MainTabControl.Items.Add(new TabItem
                {
                    Header = Messages.HomePage,
                    Content = new ViewModelViewHost { ViewModel = this.ViewModel.HomePage }
                });

                this.AddFileTabOnChange(changes => changes.Select(change => change.Item.Current), ListChangeReason.Add)
                    .DisposeWith(disposables);

                this.AddFileTabOnChange(changes => changes.SelectMany(change => change.Range), ListChangeReason.AddRange)
                    .DisposeWith(disposables);

                this.ViewModel.Files
                    .ToObservableChangeSet()
                    .WhereReasonsAre(ListChangeReason.Remove)
                    .SelectMany(changeSet => changeSet)
                    .Select(change => change.Item.Current.FileName)
                    .Select(fileName => this.MainTabControl.Items
                        .Cast<TabItem>()
                        .First(item => fileName.Equals(item.Tag)))
                    .ObserveOnDispatcher()
                    .Subscribe(item => this.MainTabControl.Items.Remove(item))
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.SelectedItemIndex, v => v.MainTabControl.SelectedIndex)
                    .DisposeWith(disposables);

                this.ViewModel.OpenFile
                    .WhereNotNull()
                    .Where(model => model.IsExternal)
                    .Discard()
                    .ObserveOnDispatcher()
                    .Subscribe(this.OnOpenFileExternally)
                    .DisposeWith(disposables);

                this.Events().Drop
                    .Where(e => e.Data.GetDataPresent(DataFormats.FileDrop))
                    .SelectMany(e => (string[])e.Data.GetData(DataFormats.FileDrop))
                    .Select(file => new OpenFileModel(file, true))
                    .InvokeCommand(this.ViewModel.OpenFile);

                this.Events().Closing
                    .Do(e => e.Cancel = true)
                    .Discard()
                    .InvokeCommand(this.ViewModel.Shutdown)
                    .DisposeWith(disposables);

                this.ViewModel.Shutdown
                    .Do(unit => disposables.Dispose())
                    .Subscribe(this.Close);
            });
        }

        private IDisposable AddFileTabOnChange(
            Func<IObservable<Change<TabItem>>, IObservable<TabItem>> itemSelector,
            params ListChangeReason[] reasons)
            => itemSelector(this.ViewModel.Files
                .ToObservableChangeSet()
                .WhereReasonsAre(reasons)
                .Transform(vm => new TabItem
                {
                    Header = new ViewModelViewHost { ViewModel = vm.Header },
                    Content = new ViewModelViewHost { ViewModel = vm },
                    Tag = vm.FileName
                })
                .SelectMany(changeSet => changeSet))
                .ObserveOnDispatcher()
                .Subscribe(item => this.MainTabControl.Items.Add(item));

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
    }
}
