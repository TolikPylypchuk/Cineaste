using System.Reactive.Disposables;

using MovieList.Core.DialogModels;

using ReactiveUI;

namespace MovieList.Dialogs
{
    public abstract class MessageDialogBase : ReactiveUserControl<MessageModel> { }

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

                this.OneWayBind(this.ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Message, v => v.MessageTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.CloseText, v => v.Button.Content, text => text?.ToUpper())
                    .DisposeWith(disposables);
            });
        }
    }
}
