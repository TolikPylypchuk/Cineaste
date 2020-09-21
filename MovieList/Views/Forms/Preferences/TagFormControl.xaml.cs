using System.Reactive;
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

            this.Bind(this.ViewModel, vm => vm.Category, v => v.CategoryTextBox.Text)
                ?.DisposeWith(disposables);

            this.CategoryTextBox.ValidateWith(this.ViewModel!.CategoryRule)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Color, v => v.ColorTextBox.Text)
                ?.DisposeWith(disposables);

            this.ColorTextBox.ValidateWith(this.ViewModel!.ColorRule)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsApplicableToMovies, v => v.IsApplicableToMoviesCheckBox.IsChecked)
                ?.DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsApplicableToSeries, v => v.IsApplicableToSeriesCheckBox.IsChecked)
                ?.DisposeWith(disposables);

            this.Bind(
                this.ViewModel,
                vm => vm.IsApplicableToFranchises,
                v => v.IsApplicableToFranchisesCheckBox.IsChecked)
                ?.DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.SaveButton.Command = DialogHost.CloseDialogCommand;
            this.SaveButton.CommandParameter = true;

            this.CancelButton.Command = DialogHost.CloseDialogCommand;
            this.CancelButton.CommandParameter = false;

            this.CloseButton.Command = DialogHost.CloseDialogCommand;
            this.CloseButton.CommandParameter = false;

            this.ViewModel!.Save.CanExecute
                .BindTo(this, v => v.SaveButton.IsEnabled)
                .DisposeWith(disposables);
        }
    }
}
