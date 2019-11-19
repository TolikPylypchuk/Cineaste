using System.Reactive.Disposables;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ListControlBase : ReactiveUserControl<ListViewModel> { }

    public partial class ListControl : ListControlBase
    {
        public ListControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Items, v => v.List.ItemsSource)
                    .DisposeWith(disposables);
            });
        }
    }
}
