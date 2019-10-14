using System;
using System.Reactive.Disposables;
using System.Windows.Media;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class RecentFileControlBase : ReactiveUserControl<RecentFileViewModel> { }

    public partial class RecentFileControl : RecentFileControlBase
    {
        public RecentFileControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(this.ViewModel, vm => vm.File.Name, v => v.ListNameTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.File.Path, v => v.ListPathTextBlock.Text)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.IsSelected, v => v.IsSelectedCheckBox.IsChecked)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel.IsSelected)
                    .Subscribe(isSelected => this.Border.Background = isSelected
                        ? (Brush)this.FindResource("MaterialDesignSelection")
                        : Brushes.Transparent)
                    .DisposeWith(disposables);
            });
        }
    }
}
