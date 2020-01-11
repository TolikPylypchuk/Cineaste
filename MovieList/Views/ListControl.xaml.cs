using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

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

                this.WhenAnyValue(v => v.List.SelectedItem)
                    .WhereNotNull()
                    .InvokeCommand(this.ViewModel.SelectItem)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.List.SelectedItem)
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .Subscribe(this.List.ScrollIntoView);

                this.WhenAnyValue(v => v.ViewModel.SelectedItem)
                    .Where(item => item == null)
                    .Subscribe(_ => this.List.SelectedItem = null)
                    .DisposeWith(disposables);

                this.ViewModel.ForceSelectedItem
                    .Merge(this.ViewModel.Save)
                    .Subscribe(() => this.List.SelectedItem = this.ViewModel.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.SideViewModel, v => v.SideViewHost.ViewModel)
                    .DisposeWith(disposables);
            });
        }
    }
}
