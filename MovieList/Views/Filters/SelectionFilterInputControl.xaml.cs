using System.Reactive.Disposables;

using MovieList.Core.ViewModels.Filters;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class SelectionFilterInputControlBase : ReactiveUserControl<SelectionFilterInputViewModel> { }

    public partial class SelectionFilterInputControl : SelectionFilterInputControlBase
    {
        public SelectionFilterInputControl()
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
