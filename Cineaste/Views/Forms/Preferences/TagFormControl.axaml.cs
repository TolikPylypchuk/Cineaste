using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms.Preferences;

using DynamicData.Aggregation;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Cineaste.Views.Forms.Preferences
{
    public partial class TagFormControl : ReactiveUserControl<TagFormViewModel>
    {
        public TagFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.BindFields(disposables);
                this.BindImpliedTags(disposables);
                this.BindValidation(disposables);
                this.BindCommands(disposables);
            });
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Description, v => v.DescriptionTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Category, v => v.CategoryTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsApplicableToMovies, v => v.IsApplicableToMoviesCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsApplicableToSeries, v => v.IsApplicableToSeriesCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Color, v => v.ColorPicker.Color)
                .DisposeWith(disposables);
        }

        private void BindImpliedTags(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.ImpliedTags, v => v.ImpliedTags.Items)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.AddableImpliedTags, v => v.AddableImpliedTagsComboBox.Items)
                .DisposeWith(disposables);

            this.AddableImpliedTagsComboBox.GetObservable(SelectingItemsControl.SelectionChangedEvent)
                .Select(e => e.AddedItems.OfType<AddableTagViewModel>().FirstOrDefault())
                .WhereNotNull()
                .Select(vm => vm.TagModel!)
                .InvokeCommand(this.ViewModel!.AddImpliedTag)
                .DisposeWith(disposables);

            this.AddableImpliedTagsComboBox.GetObservable(SelectingItemsControl.SelectionChangedEvent)
                .Where(e => this.AddableImpliedTagsComboBox.SelectedItem != null)
                .Subscribe(e => this.AddableImpliedTagsComboBox.SelectedItem = null)
                .DisposeWith(disposables);

            this.ViewModel!.AddableImpliedTags
                .ToObservableChangeSet()
                .Count()
                .StartWith(this.ViewModel.AddableImpliedTags.Count)
                .Select(count => count > 0)
                .BindTo(this, v => v.AddableImpliedTagsComboBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void BindValidation(CompositeDisposable disposables)
        {
            this.BindStrictValidation(this.ViewModel, vm => vm.Name, v => v.NameErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindStrictValidation(this.ViewModel, vm => vm.Category, v => v.CategoryErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindValidation(this.ViewModel, vm => vm!.UniqueRule, v => v.InvalidFormTextBlock.Text)
                .DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.ViewModel!.Save
                .Discard()
                .Merge(this.ViewModel.Cancel)
                .InvokeCommand(this.ViewModel!.Close)
                .DisposeWith(disposables);

            this.ViewModel.AddImpliedTag
                .Subscribe(() => this.AddableImpliedTagsComboBox.SelectedItem = null)
                .DisposeWith(disposables);
        }
    }
}
