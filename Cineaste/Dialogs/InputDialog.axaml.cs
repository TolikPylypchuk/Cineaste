namespace Cineaste.Dialogs;

public partial class InputDialog : ReactiveUserControl<InputModel>
{
    public InputDialog()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Message, v => v.CaptionTextBlock.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Value, v => v.ValueTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ConfirmText, v => v.ConfirmButton.Content)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.CancelText, v => v.CancelButton.Content)
                .DisposeWith(disposables);

            this.ConfirmButton.GetObservable(Button.ClickEvent)
                .Select(_ => true)
                .InvokeCommand(this.ViewModel!.Close)
                .DisposeWith(disposables);

            this.CancelButton.GetObservable(Button.ClickEvent)
                .Select(_ => false)
                .InvokeCommand(this.ViewModel!.Close)
                .DisposeWith(disposables);
        });
    }
}
