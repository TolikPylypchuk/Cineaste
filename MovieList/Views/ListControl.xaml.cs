using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using MovieList.Core;
using MovieList.Core.ViewModels;

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
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Items, v => v.List.ItemsSource)
                    ?.DisposeWith(disposables);

                this.WhenAnyValue(v => v.List.SelectedItem)
                    .WhereNotNull()
                    .InvokeCommand(this.ViewModel!.PreviewSelectItem)
                    ?.DisposeWith(disposables);

                this.WhenAnyValue(v => v.List.SelectedItem)
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .Subscribe(this.List.ScrollIntoView)
                    ?.DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.SelectedItem)
                    .Where(item => item == null)
                    .Subscribe(_ => this.List.SelectedItem = null)
                    ?.DisposeWith(disposables);

                this.ViewModel.ForceSelectedItem
                    .Subscribe(() => this.List.SelectedItem = this.ViewModel.SelectedItem)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
