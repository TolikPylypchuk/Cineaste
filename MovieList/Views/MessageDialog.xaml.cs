using System.Reactive.Disposables;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class MessageDialogBase : ReactiveUserControl<MessageViewModel> { }

    public partial class MessageDialog : MessageDialogBase
    {
        public MessageDialog()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Message, v => v.TextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.ButtonText, v => v.Button.Content, text => text.ToUpper())
                    .DisposeWith(disposables);
            });
        }
    }
}
