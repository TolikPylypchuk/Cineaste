using System.Reactive.Disposables;

using MovieList.Core.ViewModels.Filters;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class RangeFilterInputControlBase : ReactiveUserControl<RangeFilterInputViewModel> { }

    public partial class RangeFilterInputControl : RangeFilterInputControlBase
    {
        public RangeFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
