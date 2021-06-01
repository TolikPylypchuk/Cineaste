using System.Reactive.Disposables;

using Cineaste.Core.ViewModels.Forms;
using Cineaste.Properties;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

namespace Cineaste.Views.Forms
{
    public abstract class TitleFormControlBase : ReactiveUserControl<TitleFormViewModel> { }

    public partial class TitleFormControl : TitleFormControlBase
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

                this.NameTextBox.ValidateWith(this.ViewModel!.NameRule)
                    .DisposeWith(disposables);

                HintAssist.SetHint(
                    this.NameTextBox,
                    this.ViewModel.Title.IsOriginal ? Messages.OriginalTitle : Messages.Title);

                this.BindCommand(this.ViewModel!, vm => vm.Delete, v => v.DeleteButton)
                    .DisposeWith(disposables);

                this.ViewModel.Delete.CanExecute
                    .BindTo(this, v => v.DeleteButton.Visibility)
                    .DisposeWith(disposables);
            });
        }
    }
}
