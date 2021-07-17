using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms;
using Cineaste.Core.ViewModels.Forms.Preferences;

using DynamicData.Binding;

using ReactiveUI;

namespace Cineaste.Views.Forms
{
    public partial class MovieFormControl : ReactiveUserControl<MovieFormViewModel>
    {
        public MovieFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.ViewModel!.FormTitle
                    .BindTo(this, v => v.FormTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.LoadPoster();

                this.BindCommands(disposables);
                this.BindCheckboxes(disposables);
                this.BindLinks(disposables);
                this.BindFields(disposables);
                this.BindTags(disposables);

                this.AddValidation(disposables);
            });
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToFranchise, v => v.GoToFranchiseButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToFranchise.CanExecute
                .BindTo(this, v => v.GoToFranchiseButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToFranchise, v => v.GoToFranchiseArrowButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToFranchise.CanExecute
                .BindTo(this, v => v.GoToFranchiseArrowButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToNext, v => v.GoToNextButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToNext.CanExecute
                .BindTo(this, v => v.GoToNextButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToPrevious, v => v.GoToPreviousButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToPrevious.CanExecute
                .BindTo(this, v => v.GoToPreviousButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.CreateFranchise, v => v.CreateFranchiseButton)
                .DisposeWith(disposables);

            this.ViewModel!.CreateFranchise.CanExecute
                .BindTo(this, v => v.CreateFranchiseButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.ViewModel!.Delete.CanExecute
                .BindTo(this, v => v.DeleteButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.AddTitle, v => v.AddTitleButton)
                .DisposeWith(disposables);

            this.ViewModel!.AddTitle.CanExecute
                .BindTo(this, v => v.AddTitleButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.AddOriginalTitle, v => v.AddOriginalTitleButton)
                .DisposeWith(disposables);

            this.ViewModel!.AddOriginalTitle.CanExecute
                .BindTo(this, v => v.AddOriginalTitleButton.IsVisible)
                .DisposeWith(disposables);

            Observable.CombineLatest(this.ViewModel!.Save.CanExecute, this.ViewModel!.Cancel.CanExecute)
                .AnyTrue()
                .BindTo(this, v => v.ActionPanel.IsVisible)
                .DisposeWith(disposables);

            this.ViewModel!.Save
                .Subscribe(_ => this.LoadPoster())
                .DisposeWith(disposables);
        }

        private void BindCheckboxes(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.IsWatched, v => v.IsWatchedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsReleased, v => v.IsReleasedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Year)
                .Select(year => year <= DateTime.Now.Year)
                .BindTo(this, v => v.IsWatchedCheckBox.IsEnabled)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Year)
                .Select(year => year == DateTime.Now.Year)
                .BindTo(this, v => v.IsReleasedCheckBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void BindLinks(CompositeDisposable disposables)
        {
            this.ImdbLinkButton.GetObservable(Button.ClickEvent)
                .Select(_ => this.ViewModel!.ImdbLink)
                .WhereNotNull()
                .Subscribe(link => new Uri(link).OpenInBrowser())
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.ImdbLink)
                .Select(link => !String.IsNullOrWhiteSpace(link))
                .BindTo(this, v => v.ImdbLinkButton.IsVisible)
                .DisposeWith(disposables);

            this.RottenTomatoesLinkButton.GetObservable(Button.ClickEvent)
                .Select(_ => this.ViewModel!.RottenTomatoesLink)
                .WhereNotNull()
                .Subscribe(link => new Uri(link).OpenInBrowser())
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.RottenTomatoesLink)
                .Select(link => !String.IsNullOrWhiteSpace(link))
                .BindTo(this, v => v.RottenTomatoesLinkButton.IsVisible)
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.Items)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.Items)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Year, v => v.YearTextBox.Value)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Kinds, v => v.KindComboBox.Items)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Kind, v => v.KindComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLinkTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);
        }

        private void BindTags(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Tags, v => v.Tags.Items)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.AddableTags, v => v.AddableTagsComboBox.Items)
                .DisposeWith(disposables);

            this.AddableTagsComboBox.GetObservable(SelectingItemsControl.SelectionChangedEvent)
                .Select(e => e.AddedItems.OfType<AddableTagViewModel>().FirstOrDefault())
                .WhereNotNull()
                .Select(vm => vm.Tag)
                .InvokeCommand(this.ViewModel!.AddTag)
                .DisposeWith(disposables);

            this.AddableTagsComboBox.GetObservable(SelectingItemsControl.SelectionChangedEvent)
                .Where(e => this.AddableTagsComboBox.SelectedItem != null)
                .Subscribe(e => this.AddableTagsComboBox.SelectedItem = null)
                .DisposeWith(disposables);

            this.ViewModel!.AddableTags
                .ToObservableChangeSet()
                .Count()
                .StartWith(this.ViewModel.AddableTags.Count)
                .Select(count => count > 0)
                .BindTo(this, v => v.AddableTagsComboBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables)
        {
            this.BindDefaultValidation(this.ViewModel, vm => vm.Year, v => v.YearErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindDefaultValidation(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLinkErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindDefaultValidation(
                this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindDefaultValidation(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlErrorTextBlock.Text)
                .DisposeWith(disposables);
        }

        private void LoadPoster()
        {
            if (!String.IsNullOrEmpty(this.ViewModel!.PosterUrl))
            {
                BlobCache.UserAccount.DownloadUrl(this.ViewModel!.PosterUrl, TimeSpan.FromMinutes(5))
                    .Select(data => data.AsImage())
                    .WhereNotNull()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.Poster.Source);
            } else
            {
                this.Poster.Source = null;
            }
        }
    }
}
