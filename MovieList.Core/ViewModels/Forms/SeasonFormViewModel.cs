using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static MovieList.Data.Constants;

namespace MovieList.Core.ViewModels.Forms
{
    public sealed class SeasonFormViewModel : SeriesComponentFormBase<Season, SeasonFormViewModel>
    {
        private readonly SourceList<Period> periodsSource = new SourceList<Period>();

        private readonly ReadOnlyObservableCollection<PeriodFormViewModel> periods;

        public SeasonFormViewModel(
            Season season,
            SeriesFormViewModel parent,
            IObservable<int> maxSequenceNumber,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(parent, maxSequenceNumber, resourceManager, scheduler)
        {
            this.Season = season;

            var canDeletePeriod = this.periodsSource.Connect()
                .Count()
                .Select(count => count > MinPeriodCount);

            this.periodsSource.Connect()
                .Sort(SortExpressionComparer<Period>.Ascending(period => period.StartYear)
                    .ThenByAscending(period => period.StartMonth)
                    .ThenByAscending(period => period.EndYear)
                    .ThenByAscending(period => period.EndMonth))
                .Transform(period => this.CreatePeriodForm(period, canDeletePeriod))
                .Bind(out this.periods)
                .DisposeMany()
                .Subscribe();

            this.CopyProperties();

            this.ChannelRule = this.ValidationRule(
                vm => vm.Channel, channel => !String.IsNullOrWhiteSpace(channel), "ChannelEmpty");

            this.PeriodsNonOverlapping =
                this.periods.ToObservableChangeSet()
                    .AutoRefreshOnObservable(pvm => pvm.Changed)
                    .Select(_ => this.AreAllPeriodsNonOverlapping());

            this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Select(index => this.Season.Periods[index].PosterUrl)
                .BindTo(this, vm => vm.CurrentPosterUrl);

            var canAddPeriod = this.Periods.ToObservableChangeSet()
                .AutoRefreshOnObservable(period => period.Valid)
                .ToCollection()
                .Select(periods => periods.Count < MaxPeriodCount && periods.All(period => !period.HasErrors))
                .CombineLatest(this.PeriodsNonOverlapping, (a, b) => a && b);

            this.AddPeriod = ReactiveCommand.Create(this.OnAddPeriod, canAddPeriod);

            var canSwitchToNextPoster = this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Merge(this.Save.Select(_ => this.CurrentPosterIndex))
                .Select(index =>
                    index != this.Season.Periods.Count - 1 &&
                    this.Season.Periods.Skip(index + 1).Any(period => period.PosterUrl != null));

            var canSwitchToPreviousPoster = this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Merge(this.Save.Select(_ => this.CurrentPosterIndex))
                .Select(index =>
                    index != 0 &&
                    this.Season.Periods.Take(index).Any(period => period.PosterUrl != null));

            this.SwitchToNextPoster = ReactiveCommand.Create(
                () => this.SetCurrentPosterIndex(index => index + 1), canSwitchToNextPoster);

            this.SwitchToPreviousPoster = ReactiveCommand.Create(
                () => this.SetCurrentPosterIndex(index => index - 1), canSwitchToPreviousPoster);

            this.Save.Discard()
                .Merge(this.GoToSeries.Discard())
                .Delay(TimeSpan.FromMilliseconds(500), this.Scheduler)
                .Subscribe(() => this.CurrentPosterIndex = 0);

            this.CanAlwaysDelete();
            this.EnableChangeTracking();
        }

        public Season Season { get; }

        [Reactive]
        public SeasonWatchStatus WatchStatus { get; set; }

        [Reactive]
        public SeasonReleaseStatus ReleaseStatus { get; set; }

        [Reactive]
        public override string Channel { get; set; } = String.Empty;

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

        public override int GetNextYear()
            => Int32.Parse(this.Periods.Last().EndYear) + 1;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.WatchStatus, vm => vm.Season.WatchStatus);
            this.TrackChanges(vm => vm.ReleaseStatus, vm => vm.Season.ReleaseStatus);
            this.TrackChanges(vm => vm.Channel, vm => vm.Season.Channel);
            this.TrackChanges(vm => vm.SequenceNumber, vm => vm.Season.SequenceNumber);
            this.TrackChanges(this.IsCollectionChanged(vm => vm.Periods, vm => vm.Season.Periods));

            this.TrackValidation(this.IsCollectionValid(this.Periods));
            this.TrackValidation(this.PeriodsNonOverlapping);

            base.EnableChangeTracking();
        }

        protected override IObservable<Season> OnSave()
            => this.SaveTitles()
                .DoAsync(this.SavePeriods)
                .Select(() =>
                {
                    this.Season.WatchStatus = this.WatchStatus;
                    this.Season.ReleaseStatus = this.ReleaseStatus;
                    this.Season.Channel = this.Channel;
                    this.Season.SequenceNumber = this.SequenceNumber;

                    this.SetCurrentPosterIndexToFirst();

                    return this.Season;
                });

        protected override IObservable<Season?> OnDelete()
            => this.PromptToDelete("DeleteSeason", () => Observable.Return(this.Season));

        protected override void CopyProperties()
        {
            base.CopyProperties();

            this.periodsSource.Edit(periods =>
            {
                periods.Clear();
                periods.AddRange(this.Season.Periods);
            });

            this.WatchStatus = this.Season.WatchStatus;
            this.ReleaseStatus = this.Season.ReleaseStatus;
            this.Channel = this.Season.Channel;
            this.SequenceNumber = this.Season.SequenceNumber;

            this.SetCurrentPosterIndexToFirst();
        }

        protected override void AttachTitle(Title title)
            => title.Season = this.Season;

        private PeriodFormViewModel CreatePeriodForm(Period period, IObservable<bool> canDelete)
        {
            var periodForm = new PeriodFormViewModel(period, canDelete, this.ResourceManager, this.Scheduler);

            periodForm.Delete
                .WhereNotNull()
                .Subscribe(_ => this.CurrentPosterIndex = 0);

            periodForm.Delete
                .WhereNotNull()
                .Subscribe(deletedPeriod => this.periodsSource.Remove(deletedPeriod));

            return periodForm;
        }

        private void OnAddPeriod()
        {
            string lastYear = this.Periods.OrderByDescending(period => period.StartYear)
                .ThenByDescending(period => period.StartMonth)
                .ThenByDescending(period => period.EndYear)
                .ThenByDescending(period => period.EndMonth)
                .First()
                .EndYear;

            int year = Int32.Parse(lastYear) + 1;

            this.periodsSource.Add(new Period
            {
                StartMonth = 1,
                StartYear = year,
                EndMonth = 1,
                EndYear = year,
                NumberOfEpisodes = 1,
                Season = this.Season
            });
        }

        private IObservable<Unit> SavePeriods()
            => this.Periods
                .Select(period => period.Save.Execute())
                .ForkJoin()
                .Discard()
                .Do(() =>
                {
                    foreach (var period in this.periodsSource.Items.Except(this.Season.Periods).ToList())
                    {
                        this.Season.Periods.Add(period);
                    }

                    foreach (var period in this.Season.Periods.Except(this.periodsSource.Items).ToList())
                    {
                        this.Season.Periods.Remove(period);
                    }
                });

        private void SetCurrentPosterIndexToFirst()
        {
            int oldIndex = this.CurrentPosterIndex;

            var firstPeriodWithPoster = this.Periods.FirstOrDefault(period => period.PosterUrl != null);

            this.CurrentPosterIndex = firstPeriodWithPoster != null
                ? this.Periods.IndexOf(firstPeriodWithPoster)
                : 0;

            if (this.CurrentPosterIndex == oldIndex)
            {
                this.CurrentPosterUrl = firstPeriodWithPoster?.PosterUrl;
            }
        }

        private void SetCurrentPosterIndex(Func<int, int> next)
            => this.CurrentPosterIndex = this.GetPosterIndex(next(this.CurrentPosterIndex), next);

        private int GetPosterIndex(int index, Func<int, int> next)
            => this.Periods[index].Period.PosterUrl != null
                ? index
                : this.GetPosterIndex(next(index), next);

        private bool AreAllPeriodsNonOverlapping()
            => this.Periods.Count == 1 ||
               this.Periods.Buffer(2, 1)
                   .Where(periods => periods.Count == 2)
                   .All(periods => this.ArePeriodsNonOverlapping(periods[0], periods[1]));

        private bool ArePeriodsNonOverlapping(PeriodFormViewModel earlier, PeriodFormViewModel later)
            => Int32.TryParse(earlier.EndYear, out int earlierEndYear) &&
               Int32.TryParse(later.StartYear, out int laterStartYear) &&
               (earlierEndYear < laterStartYear ||
                earlierEndYear == laterStartYear && earlier.EndMonth <= later.StartMonth);
    }
}
