using System.Reactive.Disposables;
using System.Reactive.Linq;

using MaterialDesignThemes.Wpf;

using MovieList.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class TagFormControlBase : ReactiveUserControl<TagFormViewModel> { }

    public partial class TagFormControl : TagFormControlBase
    {
        public TagFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.BindFields(disposables);
                this.BindCommands(disposables);
            });
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.FormTitle, v => v.TitleTextBlock.Text)
                ?.DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                ?.DisposeWith(disposables);

            this.NameTextBox.ValidateWith(this.ViewModel!.NameRule)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Description, v => v.DescriptionTextBox.Text)
                ?.DisposeWith(disposables);

            this.DescriptionTextBox.ValidateWith(this.ViewModel!.DescriptionRule)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Category, v => v.CategoryTextBox.Text)
                ?.DisposeWith(disposables);

            this.CategoryTextBox.ValidateWith(this.ViewModel!.CategoryRule)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Color, v => v.ColorTextBox.Text)
                ?.DisposeWith(disposables);

            this.ColorTextBox.ValidateWith(this.ViewModel!.ColorRule)
                .DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveButton)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Cancel, v => v.CancelButton)
                ?.DisposeWith(disposables);

            this.ViewModel?.Save
                .Select(_ => this.ViewModel)
                .InvokeCommand(DialogHost.CloseDialogCommand)
                ?.DisposeWith(disposables);

            this.ViewModel?.Cancel
                .Select(_ => (TagFormViewModel?)null)
                .InvokeCommand(DialogHost.CloseDialogCommand)
                ?.DisposeWith(disposables);
        }
    }
}
