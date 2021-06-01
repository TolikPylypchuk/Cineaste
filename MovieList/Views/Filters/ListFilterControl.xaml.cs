using System.Reactive.Disposables;

using Cineaste.Core.ViewModels.Filters;

using ReactiveUI;

namespace Cineaste.Views.Filters
{
    public abstract class ListFilterControlBase : ReactiveUserControl<ListFilterViewModel> { }

    public partial class ListFilterControl : ListFilterControlBase
    {
        public ListFilterControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.FilterItem, v => v.FilterItemViewHost.ViewModel)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.Apply, v => v.ApplyFilterButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.Clear, v => v.ClearFilterButton)
                    .DisposeWith(disposables);
            });
        }
    }
}
