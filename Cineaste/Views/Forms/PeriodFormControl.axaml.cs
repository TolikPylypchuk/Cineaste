using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms;
using Cineaste.Properties;

using ReactiveUI;
using ReactiveUI.Validation.Extensions;

using static Cineaste.Data.Constants;

namespace Cineaste.Views.Forms
{
    public partial class PeriodFormControl : ReactiveUserControl<PeriodFormViewModel>
    {
        public PeriodFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.BindLink(disposables);
                this.BindFields(disposables);
                this.AddValidation(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.Delete, v => v.DeleteButton)
                    .DisposeWith(disposables);

                this.ViewModel!.Delete.CanExecute
                    .BindTo(this, v => v.DeleteButton.IsVisible)
                    .DisposeWith(disposables);
            });
        }

        private void BindLink(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkButton.NavigateUri)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.RottenTomatoesLink)
                .Select(link => !String.IsNullOrWhiteSpace(link))
                .BindTo(this, v => v.RottenTomatoesLinkButton.IsVisible)
                .DisposeWith(disposables);
        }

        public void BindFields(CompositeDisposable disposables)
        {
            var isUpdatingStartDateChanged = new Subject<Unit>();

            this.WhenAnyValue(
                v => v.ViewModel!.StartYear,
                v => v.ViewModel!.StartMonth,
                (year, month) => new DateTimeOffset(new DateTime(year, month, 1)))
                .SkipUntil(isUpdatingStartDateChanged.StartWith(Unit.Default))
                .TakeUntil(isUpdatingStartDateChanged)
                .Repeat()
                .BindTo(this, v => v.StartDatePicker.SelectedDate)
                .DisposeWith(disposables);

            this.StartDatePicker.GetObservable(DatePicker.SelectedDateProperty)
                .WhereValueNotNull()
                .Subscribe(date =>
                {
                    isUpdatingStartDateChanged.OnNext(Unit.Default);
                    this.ViewModel!.StartMonth = date.Month;
                    this.ViewModel!.StartYear = date.Year;
                    isUpdatingStartDateChanged.OnNext(Unit.Default);
                })
                .DisposeWith(disposables);

            var isUpdatingEndDateChanged = new Subject<Unit>();

            this.WhenAnyValue(
                v => v.ViewModel!.EndYear,
                v => v.ViewModel!.EndMonth,
                (year, month) => new DateTimeOffset(new DateTime(year, month, 1)))
                .SkipUntil(isUpdatingEndDateChanged.StartWith(Unit.Default))
                .TakeUntil(isUpdatingEndDateChanged)
                .Repeat()
                .BindTo(this, v => v.EndDatePicker.SelectedDate)
                .DisposeWith(disposables);

            this.EndDatePicker.GetObservable(DatePicker.SelectedDateProperty)
                .WhereValueNotNull()
                .Subscribe(date =>
                {
                    isUpdatingEndDateChanged.OnNext(Unit.Default);
                    this.ViewModel!.EndMonth = date.Month;
                    this.ViewModel!.EndYear = date.Year;
                    isUpdatingEndDateChanged.OnNext(Unit.Default);
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.IsSingleDayRelease)
                .Invert()
                .BindTo(this, v => v.EndDatePanel.IsVisible)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.IsSingleDayRelease)
                .Select(isSingleDayRelease => isSingleDayRelease ? Messages.Release : Messages.Start)
                .BindTo(this, v => v.StartDateCaption.Text)
                .DisposeWith(disposables);

            this.NumberOfEpisodesBox.Minimum = PeriodMinNumberOfEpisodes;
            this.NumberOfEpisodesBox.Maximum = PeriodMaxNumberOfEpisodes;

            this.Bind(this.ViewModel, vm => vm.NumberOfEpisodes, v => v.NumberOfEpisodesBox.Value)
                .DisposeWith(disposables);

            this.NumberOfEpisodesBox.GetObservable(RequestBringIntoViewEvent)
                .Subscribe(e => e.Handled = true)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsSingleDayRelease, v => v.IsSingleDayReleaseCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.WhenAnyValue(
                    v => v.ViewModel!.StartMonth,
                    v => v.ViewModel!.StartYear,
                    v => v.ViewModel!.EndMonth,
                    v => v.ViewModel!.EndYear,
                    (startMonth, startYear, endMonth, endYear) => startMonth == endMonth && startYear == endYear)
                .BindTo(this, v => v.IsSingleDayReleaseCheckBox.IsEnabled)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.ShowPosterUrl)
                .BindTo(this, v => v.PosterUrlGrid.IsVisible)
                .DisposeWith(disposables);
        }

        public void AddValidation(CompositeDisposable disposables)
        {
            this.BindValidation(this.ViewModel, vm => vm.StartYear, v => v.StartDateErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindValidation(this.ViewModel, vm => vm.EndYear, v => v.EndDateErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindValidation(this.ViewModel, vm => vm.NumberOfEpisodes, v => v.NumberOfEpisodesErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindValidation(
                this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindValidation(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindValidation(this.ViewModel, vm => vm!.ValidPeriodRule, v => v.InvalidFormTextBlock.Text)
                .DisposeWith(disposables);
        }
    }
}
