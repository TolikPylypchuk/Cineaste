using System.Reactive.Disposables;

using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms;

using ReactiveUI;

namespace Cineaste.Views.Forms
{
    public partial class SeriesComponentFormControl : ReactiveUserControl<SeriesComponentViewModel>
    {
        public SeriesComponentFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Years, v => v.YearsTextBlock.Text)
                    .DisposeWith(disposables);

                this.GetObservable(PointerReleasedEvent)
                    .Discard()
                    .InvokeCommand(this.ViewModel!.Select)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.MoveUp, v => v.MoveUpMenuItem)
                    .DisposeWith(disposables);

                this.ViewModel.MoveUp.CanExecute
                    .BindTo(this, v => v.MoveUpMenuItem.IsVisible)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.MoveDown, v => v.MoveDownMenuItem)
                    .DisposeWith(disposables);

                this.ViewModel.MoveDown.CanExecute
                    .BindTo(this, v => v.MoveDownMenuItem.IsVisible)
                    .DisposeWith(disposables);
            });
        }
    }
}
