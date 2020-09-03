using System.Reactive.Disposables;

using MovieList.Core.DialogModels;

using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace MovieList.Dialogs
{
    public abstract class ColorDialogBase : ReactiveUserControl<ColorModel> { }

    public partial class ColorDialog : ColorDialogBase
    {
        public ColorDialog()
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

                this.Bind(this.ViewModel, vm => vm.Color, v => v.ColorTextBox.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Color, v => v.ConfirmButton.CommandParameter)
                    ?.DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel, vm => vm.ConfirmText, v => v.ConfirmButton.Content, text => text?.ToUpper())
                    ?.DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel, vm => vm.CancelText, v => v.CancelButton.Content, text => text?.ToUpper())
                    ?.DisposeWith(disposables);

                this.ViewModel.IsValid()
                    ?.BindTo(this, v => v.ConfirmButton.IsEnabled)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
