using System.Reactive.Disposables;

using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class FranchiseAddableItemControlBase : ReactiveUserControl<FranchiseAddableItemViewModel> { }

    public partial class FranchiseAddableItemControl : FranchiseAddableItemControlBase
    {
        public FranchiseAddableItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.OriginalTitle, v => v.OriginalTitleTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Year, v => v.YearTextBlock.Text, year => $" ({year})")
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Tag, v => v.TagTextBlock.Text, tag => $" - {tag.Localized()}")
                    ?.DisposeWith(disposables);
            });
        }
    }
}
