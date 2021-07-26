using System.Reactive.Disposables;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.DialogModels;

using ReactiveUI;

namespace Cineaste.Dialogs
{
    public partial class MessageDialog : ReactiveUserControl<MessageModel>
    {
        public MessageDialog()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Message, v => v.MessageTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.CloseText, v => v.CloseButton.Content)
                    .DisposeWith(disposables);

                this.CloseButton.GetObservable(Button.ClickEvent)
                    .Discard()
                    .InvokeCommand(this.ViewModel!.Close)
                    .DisposeWith(disposables);
            });
        }
    }
}
