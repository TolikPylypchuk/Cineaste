using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

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

                this.WhenAnyValue(v => v.ViewModel.AllChildren)
                    .Select(vms => vms.Select(this.CreateTabItem))
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.MainTabControl.ItemsSource)
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
            });
        }

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

        private TabItem CreateTabItem(ReactiveObject vm)
            => new TabItem
            {
                Header = vm is FileViewModel fvm
                    ? (object)new ViewModelViewHost { ViewModel = fvm.Header }
                    : Messages.HomePage,
                Content = new ViewModelViewHost { ViewModel = vm }
            };
    }
}
