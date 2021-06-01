using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;

using Akavache;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Cineaste.Views.Forms
{
    public abstract class FranchiseFormControlBase : ReactiveUserControl<FranchiseFormViewModel> { }

    public partial class FranchiseFormControl : FranchiseFormControlBase
    {
        public FranchiseFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.WhenAnyObservable(v => v.ViewModel!.FormTitle)
                    .BindTo(this, v => v.FormTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.LoadPoster();

                this.BindCommands(disposables);
                this.BindCheckboxes(disposables);
                this.BindFields(disposables);
                this.BindItemSources(disposables);

                this.AddValidation(disposables);
            });
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            const BooleanToVisibilityHint useHidden = BooleanToVisibilityHint.UseHidden;

            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToFranchise, v => v.GoToFranchiseButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToFranchise.CanExecute
                .BindTo(this, v => v.GoToFranchiseButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToFranchise, v => v.GoToFranchiseArrowButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToFranchise.CanExecute
                .BindTo(this, v => v.GoToFranchiseArrowButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToNext, v => v.GoToNextButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToNext.CanExecute
                .BindTo(this, v => v.GoToNextButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToPrevious, v => v.GoToPreviousButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToPrevious.CanExecute
                .BindTo(this, v => v.GoToPreviousButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddMovie, v => v.AddMovieButton)
                .DisposeWith(disposables);

            this.ViewModel.AddMovie.CanExecute
                .BindTo(this, v => v.AddMovieButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddSeries, v => v.AddSeriesButton)
                .DisposeWith(disposables);

            this.ViewModel.AddSeries.CanExecute
                .BindTo(this, v => v.AddSeriesButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddFranchise, v => v.AddFranchiseButton)
                .DisposeWith(disposables);

            this.ViewModel.AddFranchise.CanExecute
                .BindTo(this, v => v.AddFranchiseButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.CreateFranchise, v => v.CreateFranchiseButton)
                .DisposeWith(disposables);

            this.ViewModel.CreateFranchise.CanExecute
                .BindTo(this, v => v.CreateFranchiseButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.ViewModel.Delete.CanExecute
                .BindTo(this, v => v.DeleteButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddTitle, v => v.AddTitleButton)
                .DisposeWith(disposables);

            this.ViewModel.AddTitle.CanExecute
                .BindTo(this, v => v.AddTitleButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddOriginalTitle, v => v.AddOriginalTitleButton)
                .DisposeWith(disposables);

            this.ViewModel.AddOriginalTitle.CanExecute
                .BindTo(this, v => v.AddOriginalTitleButton.Visibility)
                .DisposeWith(disposables);

            Observable.CombineLatest(this.ViewModel.Save.CanExecute, this.ViewModel.Cancel.CanExecute)
                .AnyTrue()
                .BindTo(this, v => v.ActionPanel.Visibility)
                .DisposeWith(disposables);

            this.ViewModel.Save
                .Subscribe(_ => this.LoadPoster())
                .DisposeWith(disposables);

            this.ViewModel.AddExistingItem
                .Subscribe(_ => this.AddableItemsComboBox.SelectedItem = null)
                .DisposeWith(disposables);
        }

        private void BindCheckboxes(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.HasTitles, v => v.HasTitlesCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ShowTitles, v => v.ShowInListCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsLooselyConnected, v => v.IsLooselyConnectedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.MergeDisplayNumbers, v => v.MergeDisplayNumbersCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.WhenAnyValue(
                    v => v.ViewModel!.HasTitles,
                    v => v.ViewModel!.CanShowTitles,
                    (hasTitles, canShowTitles) => hasTitles && canShowTitles)
                .BindTo(this, v => v.ShowInListCheckBox.IsEnabled)
                .DisposeWith(disposables);

            this.ViewModel!.Entries
                .ToObservableChangeSet()
                .AutoRefresh(vm => vm.DisplayNumber)
                .ToCollection()
                .Select(vms => vms.All(vm => vm.DisplayNumber.HasValue))
                .BindTo(this, v => v.IsLooselyConnectedCheckBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.HasTitles, v => v.Titles.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.HasTitles, v => v.OriginalTitles.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.HasTitles, v => v.AddTitleButton.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.HasTitles, v => v.AddOriginalTitleButton.Visibility)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);
        }

        private void BindItemSources(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Entries, v => v.Entries.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.AddableItems, v => v.AddableItemsComboBox.ItemsSource)
                .DisposeWith(disposables);

            this.AddableItemsComboBox.Events()
                .SelectionChanged
                .Select(e => e.AddedItems.OfType<FranchiseAddableItemViewModel>().FirstOrDefault())
                .WhereNotNull()
                .Select(vm => vm.Entry)
                .InvokeCommand(this.ViewModel!.AddExistingItem)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables) =>
            this.PosterUrlTextBox.ValidateWith(this.ViewModel!.PosterUrlRule)
                .DisposeWith(disposables);

        private void LoadPoster()
        {
            if (!String.IsNullOrEmpty(this.ViewModel!.PosterUrl))
            {
                BlobCache.UserAccount.DownloadUrl(this.ViewModel!.PosterUrl, TimeSpan.FromMinutes(5))
                    .Select(data => data.AsImage())
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.Poster.Source);
            } else
            {
                this.Poster.Source = null;
            }
        }
    }
}
