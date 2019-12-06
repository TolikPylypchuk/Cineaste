using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeasonFormViewModel : TitledFormViewModelBase<Season, SeasonFormViewModel>
    {
        public SeasonFormViewModel(
            Season season,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Season = season;
            this.CopyProperties();

            this.ChannelRule = this.ValidationRule(
                vm => vm.Channel, channel => !String.IsNullOrWhiteSpace(channel), "ChannelEmpty");

            this.WhenAnyValue(vm => vm.CurrentPosterIndex)
                .Select(index => this.Season.Periods[index].PosterUrl)
                .BindTo(this, vm => vm.CurrentPosterUrl);

            this.Close = ReactiveCommand.Create(() => { });
            this.GoToSeries = ReactiveCommand.Create(() => { });

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

        [Reactive]
        public string? CurrentPosterUrl { get; set; }

        [Reactive]
        public int CurrentPosterIndex { get; private set; }

        public ValidationHelper ChannelRule { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }
        public ReactiveCommand<Unit, Unit> GoToSeries { get; }

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
    }
}
