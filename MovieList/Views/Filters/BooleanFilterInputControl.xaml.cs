using System.Reactive.Disposables;

using MovieList.Core.ViewModels.Filters;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class BooleanFilterInputControlBase : ReactiveUserControl<BooleanFilterInputViewModel> { }

    public partial class BooleanFilterInputControl : BooleanFilterInputControlBase
    {
        public BooleanFilterInputControl()
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
