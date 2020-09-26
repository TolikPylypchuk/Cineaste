using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using MaterialDesignThemes.Wpf;

using MovieList.Core;
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
                this.BindImpliedTags(disposables);
                this.BindValidation(disposables);
                this.BindCommands(disposables);
            });
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.FormTitle, v => v.TitleTextBlock.Text)
                ?.DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                ?.DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Description, v => v.DescriptionTextBox.Text)
                ?.DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Category, v => v.CategoryTextBox.Text)
                ?.DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Color, v => v.ColorTextBox.Text)
                ?.DisposeWith(disposables);

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

        private void BindImpliedTags(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.ImpliedTags, v => v.ImpliedTags.ItemsSource)
                ?.DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.AddableImpliedTags, v => v.AddableImpliedTagsComboBox.ItemsSource)
                ?.DisposeWith(disposables);

            this.AddableImpliedTagsComboBox.Events()
                .SelectionChanged
                .Select(e => e.AddedItems.OfType<AddableImpliedTagViewModel>().FirstOrDefault())
                .WhereNotNull()
                .Select(vm => vm.TagModel)
                .InvokeCommand(this.ViewModel!.AddImpliedTag)
                ?.DisposeWith(disposables);

            this.ViewModel!.AddableImpliedTags
                .ToObservableChangeSet()
                .Count()
                .StartWith(this.ViewModel.AddableImpliedTags.Count)
                .Select(count => count > 0)
                .BindTo(this, v => v.AddableImpliedTagsComboBox.IsEnabled)
                ?.DisposeWith(disposables);
        }

        private void BindValidation(CompositeDisposable disposables)
        {
            this.NameTextBox.ValidateWith(this.ViewModel!.NameRule)
                .DisposeWith(disposables);

            this.CategoryTextBox.ValidateWith(this.ViewModel!.CategoryRule)
                .DisposeWith(disposables);

            this.ColorTextBox.ValidateWith(this.ViewModel!.ColorRule)
                .DisposeWith(disposables);

            this.ShowValidationMessage(this.ViewModel.UniqueRule, v => v.InvalidFormTextBlock.Text)
                ?.DisposeWith(disposables);

            this.ViewModel.UniqueRule.ValidationChanged
                .Select(state => !state.IsValid)
                .BindTo(this, v => v.InvalidFormTextBlock.Visibility)
                ?.DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveButton)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Cancel, v => v.CancelButton)
                ?.DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Close, v => v.CloseButton)
                ?.DisposeWith(disposables);

            this.ViewModel!.Save
                .Discard()
                .Merge(this.ViewModel.Cancel)
                .Merge(this.ViewModel.Close)
                .InvokeCommand(DialogHost.CloseDialogCommand)
                .DisposeWith(disposables);

            this.ViewModel.AddImpliedTag
                .Subscribe(() => this.AddableImpliedTagsComboBox.SelectedItem = null)
                ?.DisposeWith(disposables);
        }
    }
}
