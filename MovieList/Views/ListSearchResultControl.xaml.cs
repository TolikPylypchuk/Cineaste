using System.Reactive.Disposables;

using Cineaste.Core.ViewModels;

using ReactiveUI;

namespace Cineaste.Views
{
    public abstract class ListSearchResultControlBase : ReactiveUserControl<ListSearchResultViewModel> { }

    public partial class ListSearchResultControl : ListSearchResultControlBase
    {
        public ListSearchResultControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.OriginalTitle, v => v.OriginalTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Year, v => v.YearTextBlock.Text, year => $" ({year})")
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Tag, v => v.TagTextBlock.Text, tag => $" - {tag.Localized()}")
                    .DisposeWith(disposables);
            });
        }
    }
}
