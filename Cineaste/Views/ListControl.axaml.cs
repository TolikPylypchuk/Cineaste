using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels;

using ReactiveUI;

namespace Cineaste.Views
{
    public partial class ListControl : ReactiveUserControl<ListViewModel>
    {
        public ListControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Items, v => v.List.Items)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.List.SelectedItem)
                    .WhereNotNull()
                    .InvokeCommand(this.ViewModel!.PreviewSelectItem)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.List.SelectedItem)
                    .WhereNotNull()
                    .Merge(this.ViewModel!.Find)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(this.List.ScrollIntoView)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.SelectedItem)
                    .Where(item => item == null)
                    .Subscribe(_ => this.List.SelectedItem = null)
                    .DisposeWith(disposables);

                this.ViewModel.ForceSelectedItem
                    .Subscribe(() => this.List.SelectedItem = this.ViewModel.SelectedItem)
                    .DisposeWith(disposables);
            });
        }
    }
}
