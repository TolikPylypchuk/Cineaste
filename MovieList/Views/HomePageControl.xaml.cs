using System.Reactive.Disposables;

using MovieList.Core.ViewModels;

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

                this.OneWayBind(this.ViewModel, vm => vm.RecentFilesPresent, v => v.RecentFilesList.Visibility)
                    .DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel, vm => vm.ShowRecentFiles, v => v.RecentlyOpenedFilesTextBlock.Visibility)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.ShowRecentFiles, v => v.RecentlyOpenedFilesPanel.Visibility)
                    .DisposeWith(disposables);

                this.WhenAnyValue(
                    v => v.ViewModel!.ShowRecentFiles, v => v.ViewModel!.RecentFilesPresent, (a, b) => a && !b)
                    .BindTo(this, v => v.NoRecentlyOpenedFilesTextBlock.Visibility)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.RemoveSelectedRecentFiles, v => v.RemoveButton)
                    .DisposeWith(disposables);

                this.ViewModel!.RemoveSelectedRecentFiles.CanExecute
                    .BindTo(this, v => v.RemoveButton.Visibility);

                this.BindCommand(this.ViewModel!, vm => vm.CreateFile, v => v.CreateListButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.OpenFile, v => v.OpenListButton)
                    .DisposeWith(disposables);
            });
        }
    }
}
