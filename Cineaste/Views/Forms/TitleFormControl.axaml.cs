using System.Reactive.Disposables;

using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels.Forms;
using Cineaste.Properties;

using ReactiveUI;

namespace Cineaste.Views.Forms
{
    public partial class TitleFormControl : ReactiveUserControl<TitleFormViewModel>
    {
        public TitleFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                    .DisposeWith(disposables);

                this.BindDefaultValidation(this.ViewModel, vm => vm.Name, v => v.ErrorTextBlock.Text)
                    .DisposeWith(disposables);

                this.CaptionTextBlock.Text =
                    this.ViewModel!.Title.IsOriginal ? Messages.OriginalTitle : Messages.Title;

                this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                    .DisposeWith(disposables);

                this.ViewModel.Delete.CanExecute
                    .BindTo(this, v => v.DeleteButton.IsVisible)
                    .DisposeWith(disposables);
            });
        }
    }
}
