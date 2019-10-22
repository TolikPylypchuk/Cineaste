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

                this.WhenAnyValue(v => v.ViewModel.AllViewModels)
                    .Select(vms => vms.Select(vm => new TabItem
                    {
                        Header = vm is FileViewModel fvm ? fvm.ListName : Messages.HomePage,
                        Content = new ViewModelViewHost { ViewModel = vm }
                    }))
                    .BindTo(this, v => v.MainTabControl.ItemsSource)
                    .DisposeWith(disposables);

                this.MainTabControl.SelectedIndex = 0;

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
    }
}
