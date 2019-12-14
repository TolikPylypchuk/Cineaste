using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using MovieList.Data.Models;
using MovieList.DialogModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeasonFormViewModel : SeriesComponentFormViewModelBase<Season, SeasonFormViewModel>
    {
        private readonly SourceList<Period> periodsSource = new SourceList<Period>();

        private readonly ReadOnlyObservableCollection<PeriodFormViewModel> periods;

        public SeasonFormViewModel(
            Season season,
            SeriesFormViewModel parent,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(parent, resourceManager, scheduler)
        {
            this.Season = season;
            this.CopyProperties();

            var canDeletePeriod = this.periodsSource.Connect()
                .Select(_ => this.periodsSource.Items.Count())
                .Select(count => count > MinPeriodCount);

            this.periodsSource.Connect()
                .Sort(SortExpressionComparer<Period>.Ascending(period => period.StartYear)
                    .ThenByAscending(period => period.StartMonth)
                    .ThenByAscending(period => period.EndYear)
                    .ThenByAscending(period => period.EndMonth))
                .Transform(period => this.CreatePeriodForm(period, canDeletePeriod))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.periods)
                .DisposeMany()
                .Subscribe();

            this.ChannelRule = this.ValidationRule(
                vm => vm.Channel, channel => !String.IsNullOrWhiteSpace(channel), "ChannelEmpty");

            this.PeriodsNonOverlapping =
                this.periods.ToObservableChangeSet()
                    .AutoRefreshOnObservable(pvm => pvm.Changed)
                    .Select(_ => this.AreAllPeriodsNonOverlapping());

            this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Select(index => this.Season.Periods[index].PosterUrl)
                .BindTo(this, vm => vm.CurrentPosterUrl);

            this.CanDeleteWhenNotNew();

            var canAddPeriod = this.periodsSource.Connect()
                .Select(_ => this.periods.Count < MaxPeriodCount);

            this.AddPeriod = ReactiveCommand.Create(this.OnAddPeriod, canAddPeriod);

            var canSwitchToNextPoster = this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Select(index => index != this.Season.Periods.Count - 1);

            var canSwitchToPreviousPoster = this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Select(index => index != 0);

            this.SwitchToNextPoster = ReactiveCommand.Create(
                () => { this.CurrentPosterIndex++; }, canSwitchToNextPoster);

            this.SwitchToPreviousPoster = ReactiveCommand.Create(
                () => { this.CurrentPosterIndex--; }, canSwitchToPreviousPoster);

            this.EnableChangeTracking();
        }

        public Season Season { get; }

        [Reactive]
        public SeasonWatchStatus WatchStatus { get; set; }

        [Reactive]
        public SeasonReleaseStatus ReleaseStatus { get; set; }

        [Reactive]
        public string Channel { get; set; } = null!;

        public ReadOnlyObservableCollection<PeriodFormViewModel> Periods
            => this.periods;

        [Reactive]
        public string? CurrentPosterUrl { get; set; }

        [Reactive]
        public int CurrentPosterIndex { get; private set; }

        public ValidationHelper ChannelRule { get; }

        public IObservable<bool> PeriodsNonOverlapping { get; }

        public ReactiveCommand<Unit, Unit> AddPeriod { get; }

        public ReactiveCommand<Unit, Unit> SwitchToNextPoster { get; }
        public ReactiveCommand<Unit, Unit> SwitchToPreviousPoster { get; }

        public override bool IsNew
            => this.Season.Id == default;

        protected override SeasonFormViewModel Self
            => this;

        protected override ICollection<Title> ItemTitles
            => this.Season.Titles;

        protected override string NewItemKey
            => "NewSeason";

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.WatchStatus, vm => vm.Season.WatchStatus);
            this.TrackChanges(vm => vm.ReleaseStatus, vm => vm.Season.ReleaseStatus);
            this.TrackChanges(vm => vm.Channel, vm => vm.Season.Channel);
            this.TrackChanges(vm => vm.SequenceNumber, vm => vm.Season.SequenceNumber);
            this.TrackChanges(this.IsCollectionChanged(vm => vm.Periods, vm => vm.Season.Periods));

            this.TrackValidation(this.IsCollectionValid<PeriodFormViewModel, Period>(this.Periods));
            this.TrackValidation(this.PeriodsNonOverlapping);

            base.EnableChangeTracking();
        }

        protected override async Task<Season> OnSaveAsync()
        {
            await this.SaveTitlesAsync();
            await this.SavePeriodsAsync();

            this.Season.WatchStatus = this.WatchStatus;
            this.Season.ReleaseStatus = this.ReleaseStatus;
            this.Season.Channel = this.Channel;
            this.Season.SequenceNumber = this.SequenceNumber;

            return this.Season;
        }

        protected override async Task<Season?> OnDeleteAsync()
            => await Dialog.Confirm.Handle(new ConfirmationModel("DeleteSeries"))
                ? this.Season
                : null;

        protected override void CopyProperties()
        {
            base.CopyProperties();

            this.periodsSource.Clear();
            this.periodsSource.AddRange(this.Season.Periods);

            this.WatchStatus = this.Season.WatchStatus;
            this.ReleaseStatus = this.Season.ReleaseStatus;
            this.Channel = this.Season.Channel;
            this.SequenceNumber = this.Season.SequenceNumber;

            this.CurrentPosterIndex = 0;
            this.CurrentPosterUrl = this.Season.Periods[this.CurrentPosterIndex].PosterUrl;
        }

        protected override void AttachTitle(Title title)
            => title.Season = this.Season;

        private PeriodFormViewModel CreatePeriodForm(Period period, IObservable<bool> canDelete)
        {
            var periodForm = new PeriodFormViewModel(period, canDelete, this.ResourceManager);

            periodForm.Delete
                .WhereNotNull()
                .Subscribe(deletedPeriod => this.periodsSource.Remove(deletedPeriod));

            return periodForm;
        }

        private void OnAddPeriod()
            => this.periodsSource.Add(new Period
            {
                StartMonth = 1,
                StartYear = 2000,
                EndMonth = 1,
                EndYear = 2000,
                Season = this.Season
            });

        private async Task SavePeriodsAsync()
        {
            foreach (var period in this.Periods)
            {
                await period.Save.Execute();
            }

            foreach (var period in this.periodsSource.Items.Except(this.Season.Periods).ToList())
            {
                this.Season.Periods.Add(period);
            }

            foreach (var period in this.Season.Periods.Except(this.periodsSource.Items).ToList())
            {
                this.Season.Periods.Remove(period);
            }
        }

        private bool AreAllPeriodsNonOverlapping()
            => this.Periods.Count == 1 ||
               this.Periods.Buffer(2, 1).All(periods => this.ArePeriodsNonOverlapping(periods[0], periods[1]));

        private bool ArePeriodsNonOverlapping(PeriodFormViewModel earlier, PeriodFormViewModel later)
            => Int32.TryParse(earlier.EndYear, out int earlierEndYear) &&
               Int32.TryParse(later.StartYear, out int laterStartYear) &&
               (earlierEndYear < laterStartYear ||
                earlierEndYear == laterStartYear && earlier.EndMonth <= later.StartMonth);
    }
}
