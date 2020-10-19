using System.Reactive.Disposables;

using MovieList.Core.ViewModels.Filters;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class CompositeFilterItemControlBase : ReactiveUserControl<CompositeFilterItemViewModel> { }

    public partial class CompositeFilterItemControl : CompositeFilterItemControlBase
    {
        public CompositeFilterItemControl()
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
