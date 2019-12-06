using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeasonFormViewModel : TitledFormViewModelBase<Season, SeasonFormViewModel>
    {
        private readonly SourceList<Period> periodsSource;
        private readonly ReadOnlyObservableCollection<PeriodFormViewModel> periods;

        public SeasonFormViewModel(
            Season season,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Season = season;
            this.CopyProperties();

            this.periodsSource = new SourceList<Period>();

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

            this.periods.ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.IsValid())
                .Select(_ => this.Periods.Select(period => period.IsValid()).CombineLatest().AllTrue())
                .Switch()
                .ToPropertyEx(this, vm => vm.AreAllPeriodsValid);

            var arePeriodsValidAndOverlapping = this.periods.ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.Changed)
                .Select(_ => this.AreAllPeriodsValid && this.AreAllPeriodsNonOverlapping());

            this.PeriodOverlapRule = this.ValidationRule(
                _ => arePeriodsValidAndOverlapping,
                (_, isValid) => isValid ? String.Empty : this.ResourceManager.GetString("ValidationPeriodsOverlap"));

            this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Select(index => this.Season.Periods[index].PosterUrl)
                .BindTo(this, vm => vm.CurrentPosterUrl);

            this.Close = ReactiveCommand.Create(() => { });
            this.GoToSeries = ReactiveCommand.Create(() => { });

            var canAddPeriod = this.periodsSource.Connect()
                .Select(_ => this.periods.Count < MaxPeriodCount);

            this.AddPeriod = ReactiveCommand.Create(this.OnAddPeriod, canAddPeriod);

            this.MoveUp = ReactiveCommand.Create(() => { this.SequenceNumber--; });
            this.MoveDown = ReactiveCommand.Create(() => { this.SequenceNumber++; });

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

        [Reactive]
        public int SequenceNumber { get; set; }

        public ReadOnlyObservableCollection<PeriodFormViewModel> Periods
            => this.periods;

        [Reactive]
        public string? CurrentPosterUrl { get; set; }

        [Reactive]
        public int CurrentPosterIndex { get; private set; }

        public bool AreAllPeriodsValid { [ObservableAsProperty] get; }

        public ValidationHelper ChannelRule { get; }
        public ValidationHelper PeriodOverlapRule { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }
        public ReactiveCommand<Unit, Unit> GoToSeries { get; }

        public ReactiveCommand<Unit, Unit> AddPeriod { get; }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }
        public ReactiveCommand<Unit, Unit> MoveDown { get; }

        public ReactiveCommand<Unit, Unit> SwitchToNextPoster { get; }
        public ReactiveCommand<Unit, Unit> SwitchToPreviousPoster { get; }

        public override bool IsNew
            => this.Season.Id == default;

        protected override SeasonFormViewModel Self
            => this;

        protected override IEnumerable<Title> ItemTitles
            => this.Season.Titles;

        protected override string NewItemKey
            => "NewSeason";

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.WatchStatus, vm => vm.Season.WatchStatus);
            this.TrackChanges(vm => vm.ReleaseStatus, vm => vm.Season.ReleaseStatus);
            this.TrackChanges(vm => vm.Channel, vm => vm.Season.Channel);
            this.TrackChanges(vm => vm.SequenceNumber, vm => vm.Season.SequenceNumber);

            this.TrackChanges(
                this.periods.ToObservableChangeSet()
                    .AutoRefreshOnObservable(vm => vm.FormChanged)
                    .ToCollection()
                    .Select(vms =>
                        vms.Count == 0 ||
                        vms.Count != this.Season.Periods.Count ||
                        vms.Any(vm => vm.IsFormChanged || !this.IsNew && vm.IsNew))
                    .Do(changed => this.Log().Debug(changed ? "Periods are changed" : "Periods are unchanged")));

            this.TrackValidation(
                this.periods.ToObservableChangeSet()
                    .AutoRefreshOnObservable(vm => vm.IsValid())
                    .ToCollection()
                    .SelectMany(vms => vms.Select(vm => vm.IsValid()).CombineLatest().AllTrue()));

            base.EnableChangeTracking();
        }

        protected override Task<Season> OnSaveAsync()
        {
            this.Season.WatchStatus = this.WatchStatus;
            this.Season.ReleaseStatus = this.ReleaseStatus;
            this.Season.Channel = this.Channel;
            this.Season.SequenceNumber = this.SequenceNumber;

            return Task.FromResult(this.Season);
        }

        [SuppressMessage("ReSharper", "RedundantCast")]
        protected override Task<Season?> OnDeleteAsync()
            => Task.FromResult((Season?)this.Season);

        protected override void CopyProperties()
        {
            this.TitlesSource.Clear();
            this.TitlesSource.AddRange(this.Season.Titles);

            this.WatchStatus = this.Season.WatchStatus;
            this.ReleaseStatus = this.Season.ReleaseStatus;
            this.Channel = this.Channel;
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

        private bool AreAllPeriodsNonOverlapping()
            => this.Periods.Count == MinPeriodCount ||
               this.Periods.Buffer(2, 1).All(periods => this.ArePeriodsNonOverlapping(periods[0], periods[1]));

        private bool ArePeriodsNonOverlapping(PeriodFormViewModel earlier, PeriodFormViewModel later)
            => Int32.TryParse(earlier.EndYear, out int earlierEndYear) &&
               Int32.TryParse(later.StartYear, out int laterStartYear) &&
               (earlierEndYear < laterStartYear ||
                earlierEndYear == laterStartYear && earlier.EndMonth <= later.StartMonth);
    }
}
