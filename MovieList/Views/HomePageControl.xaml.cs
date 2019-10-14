using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class HomePageControlBase : ReactiveUserControl<HomePageViewModel> { }

    public partial class HomePageControl : HomePageControlBase
    {
        public HomePageControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.RecentFiles, v => v.RecentFilesList.ItemsSource)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel.RecentFilesPresent)
                    .Select(this.BoolToVisibility)
                    .BindTo(this, v => v.RecentFilesList.Visibility)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel.RecentFilesPresent)
                    .Select(value => !value)
                    .Select(this.BoolToVisibility)
                    .BindTo(this, v => v.NoRecentlyOpenedFilesTextBlock.Visibility)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.RemoveSelectedRecentFiles, v => v.RemoveButton)
                    .DisposeWith(disposables);

                this.ViewModel.RemoveSelectedRecentFiles.CanExecute
                    .Select(this.BoolToVisibility)
                    .BindTo(this, v => v.RemoveButton.Visibility);

                this.BindCommand(this.ViewModel, vm => vm.CreateFile, v => v.CreateListButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.OpenFile, v => v.OpenListButton)
                    .DisposeWith(disposables);
            });
        }

        private Visibility BoolToVisibility(bool value)
            => value ? Visibility.Visible : Visibility.Collapsed;
    }
}
