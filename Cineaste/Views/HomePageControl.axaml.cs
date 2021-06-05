using System.Reactive.Disposables;

using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels;

using ReactiveUI;

namespace Cineaste.Views
{
    public partial class HomePageControl : ReactiveUserControl<HomePageViewModel>
    {
        public HomePageControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.RecentFiles, v => v.RecentFilesList.Items)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.RecentFilesPresent, v => v.RecentFilesList.IsVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel, vm => vm.ShowRecentFiles, v => v.RecentlyOpenedFilesTextBlock.IsVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.ShowRecentFiles, v => v.RecentlyOpenedFilesPanel.IsVisible)
                    .DisposeWith(disposables);

                this.WhenAnyValue(
                    v => v.ViewModel!.ShowRecentFiles, v => v.ViewModel!.RecentFilesPresent, (a, b) => a && !b)
                    .BindTo(this, v => v.NoRecentlyOpenedFilesTextBlock.IsVisible)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.RemoveSelectedRecentFiles, v => v.RemoveButton)
                    .DisposeWith(disposables);

                this.ViewModel!.RemoveSelectedRecentFiles.CanExecute
                    .BindTo(this, v => v.RemoveButton.IsVisible);

                this.BindCommand(this.ViewModel, vm => vm.CreateFile, v => v.CreateListButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.OpenFile, v => v.OpenListButton)
                    .DisposeWith(disposables);
            });
        }
    }
}
