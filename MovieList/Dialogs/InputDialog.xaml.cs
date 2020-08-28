using System.Reactive.Disposables;

using MovieList.DialogModels;

using ReactiveUI;

namespace MovieList.Dialogs
{
    public abstract class InputDialogBase : ReactiveUserControl<InputModel> { }

    public partial class InputDialog : InputDialogBase
    {
        public InputDialog()
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

                this.Bind(this.ViewModel, vm => vm.Value, v => v.ValueTextBox.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Value, v => v.ConfirmButton.CommandParameter)
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
