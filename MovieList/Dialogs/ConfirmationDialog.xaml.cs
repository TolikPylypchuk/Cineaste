using System.Reactive.Disposables;

using MovieList.DialogModels;

using ReactiveUI;

namespace MovieList.Dialogs
{
    public abstract class ConfirmationDialogBase : ReactiveUserControl<ConfirmationModel> { }

    public partial class ConfirmationDialog : ConfirmationDialogBase
    {
        public ConfirmationDialog()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Message, v => v.MessageTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel, vm => vm.ConfirmText, v => v.ConfirmButton.Content, text => text?.ToUpper())
                    ?.DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel, vm => vm.CancelText, v => v.CancelButton.Content, text => text?.ToUpper())
                    ?.DisposeWith(disposables);
            });
        }
    }
}
