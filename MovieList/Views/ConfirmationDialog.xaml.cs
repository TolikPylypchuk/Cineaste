using System.Reactive.Disposables;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ConfirmationDialogBase : ReactiveUserControl<ConfirmationViewModel> { }

    public partial class ConfirmationDialog : ConfirmationDialogBase
    {
        public ConfirmationDialog()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Message, v => v.TextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel, vm => vm.ConfirmButtonText, v => v.ConfirmButton.Content, text => text.ToUpper())
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel, vm => vm.CancelButtonText, v => v.CancelButton.Content, text => text.ToUpper())
                    .DisposeWith(disposables);
            });
        }
    }
}
