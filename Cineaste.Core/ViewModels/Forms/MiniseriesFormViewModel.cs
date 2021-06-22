using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using Cineaste.Core.Data.Services;
using Cineaste.Core.ViewModels.Forms.Base;
using Cineaste.Data.Models;
using Cineaste.Data.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static Cineaste.Core.ServiceUtil;
using static Cineaste.Data.Constants;

namespace Cineaste.Core.ViewModels.Forms
{
    public sealed class MiniseriesFormViewModel : TaggedFormBase<Series, MiniseriesFormViewModel>
    {
        private readonly IEntityService<Series> seriesService;
        private readonly ISettingsService settingsService;

        public MiniseriesFormViewModel(
            Series series,
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            string fileName,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IEntityService<Series>? seriesService = null,
            ISettingsService? settingsService = null)
            : base(series.Entry, tags, resourceManager, scheduler)
        {
            this.Series = series;
            this.Kinds = kinds;

            this.seriesService = seriesService ?? GetDefaultService<IEntityService<Series>>(fileName);
            this.settingsService = settingsService ?? GetDefaultService<ISettingsService>(fileName);

            this.CopyProperties();

            this.ChannelRule = this.ValidationRule(
                vm => vm.Channel, channel => !String.IsNullOrWhiteSpace(channel), "ChannelEmpty");

            this.ImdbLinkRule = this.ValidationRule(vm => vm.ImdbLink, link => link.IsUrl(), "ImdbLinkInvalid");
            this.RottenTomatoesLinkRule = this.ValidationRule(
                vm => vm.RottenTomatoesLink, link => link.IsUrl(), "RottenTomatoesLinkInvalid");
            this.PosterUrlRule = this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

            this.ConvertToSeries = ReactiveCommand.Create(() => { });

            this.CanDeleteWhenNotChanged();
            this.CanCreateFranchise();
            this.EnableChangeTracking();
        }

        public Series Series { get; }

        public ReadOnlyObservableCollection<Kind> Kinds { get; }

        [Reactive]
        public Kind Kind { get; set; }

        [Reactive]
        public SeriesWatchStatus WatchStatus { get; set; }

        [Reactive]
        public SeriesReleaseStatus ReleaseStatus { get; set; }

        [Reactive]
        public string Channel { get; set; } = String.Empty;

        [Reactive]
        public PeriodFormViewModel PeriodForm { get; set; }

        [Reactive]
        public string ImdbLink { get; set; } = String.Empty;

        [Reactive]
        public string RottenTomatoesLink { get; set; } = String.Empty;

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        public ValidationHelper ChannelRule { get; }
        public ValidationHelper ImdbLinkRule { get; }
        public ValidationHelper RottenTomatoesLinkRule { get; }
        public ValidationHelper PosterUrlRule { get; }

        public ReactiveCommand<Unit, Unit> ConvertToSeries { get; }

        protected override MiniseriesFormViewModel Self => this;

        protected override ICollection<Title> ItemTitles =>
            this.Series.Titles;

        protected override IEnumerable<Tag> ItemTags =>
            this.Series.Tags;

        protected override string NewItemKey => "NewMiniseries";

        public override bool IsNew =>
            this.Series.Id == default;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.WatchStatus, vm => vm.Series.WatchStatus);
            this.TrackChanges(vm => vm.ReleaseStatus, vm => vm.Series.ReleaseStatus);
            this.TrackChanges(vm => vm.Kind, vm => vm.Series.Kind);
            this.TrackChanges(vm => vm.ImdbLink, vm => vm.Series.ImdbLink.EmptyIfNull());
            this.TrackChanges(vm => vm.RottenTomatoesLink, vm => vm.Series.RottenTomatoesLink.EmptyIfNull());
            this.TrackChanges(vm => vm.PosterUrl, vm => vm.Series.PosterUrl.EmptyIfNull());
            this.TrackChanges(
                vm => vm.Channel,
                vm => vm.Series.Seasons.Count == 1 ? vm.Series.Seasons[0].Channel : String.Empty);

            this.TrackChanges(this.WhenAnyValue(vm => vm.PeriodForm).Select(vm => vm!.FormChanged).Switch());

            if (!this.Series.IsMiniseries && !this.IsNew)
            {
                this.TrackChanges(Observable.Return(true).Merge(this.Save.Select(_ => false)));
            }

            base.EnableChangeTracking();
        }

        protected override IObservable<Series> OnSave() =>
            this.SaveTitles()
                .DoAsync(() => this.PeriodForm.Save.Execute())
                .Select(this.CopyPropertiesIntoModel)
                .DoAsync(this.seriesService.SaveInTaskPool);

        protected override IObservable<Series?> OnDelete() =>
            Dialog.PromptToDelete(
                "DeleteSeries", () => this.seriesService.DeleteInTaskPool(this.Series).Select(() => this.Series));

        [MemberNotNull(new[] { nameof(Kind), nameof(PeriodForm) })]
        protected override void CopyProperties()
        {
            base.CopyProperties();

            var period = this.Series.Seasons.Count == 1
                ? this.Series.Seasons[0].Periods[0]
                : new Period
                {
                    StartMonth = 1,
                    StartYear = SeriesDefaultYear,
                    EndMonth = 1,
                    EndYear = SeriesDefaultYear,
                    NumberOfEpisodes = 1
                };

            this.PeriodForm = new PeriodFormViewModel(
                period, Observable.Return(false), this.ResourceManager, this.Scheduler)
            {
                ShowPosterUrl = false
            };

            this.Kind = this.Series.Kind;
            this.WatchStatus = this.Series.WatchStatus;
            this.ReleaseStatus = this.Series.ReleaseStatus;
            this.Channel = this.Series.Seasons.Count == 1 ? this.Series.Seasons[0].Channel : String.Empty;
            this.ImdbLink = this.Series.ImdbLink.EmptyIfNull();
            this.RottenTomatoesLink = this.Series.RottenTomatoesLink.EmptyIfNull();
            this.PosterUrl = this.Series.PosterUrl.EmptyIfNull();
        }

        protected override void AttachTitle(Title title) =>
            title.Series = this.Series;

        protected override bool IsTagApplicable(Tag tag) =>
            tag.IsApplicableToSeries;

        private Series CopyPropertiesIntoModel()
        {
            this.Series.IsMiniseries = true;
            this.Series.WatchStatus = this.WatchStatus;
            this.Series.ReleaseStatus = this.ReleaseStatus;
            this.Series.Kind = this.Kind;
            this.Series.ImdbLink = this.ImdbLink.NullIfEmpty();
            this.Series.RottenTomatoesLink = this.RottenTomatoesLink.NullIfEmpty();
            this.Series.PosterUrl = this.PosterUrl.NullIfEmpty();

            if (this.Series.Seasons.Count == 0)
            {
                var settings = this.settingsService.GetSettings();

                this.Series.Seasons.Add(new Season
                {
                    Series = this.Series,
                    SequenceNumber = 1,
                    Titles = new List<Title>
                    {
                        new Title { Name = settings.GetSeasonTitle(1), Priority = 1, IsOriginal = false },
                        new Title { Name = settings.GetSeasonOriginalTitle(1), Priority = 1, IsOriginal = true }
                    },
                    Periods = new List<Period>
                    {
                        this.PeriodForm.Period
                    }
                });

                this.PeriodForm.Period.Season = this.Series.Seasons[0];
            }

            this.Series.Seasons[0].Channel = this.Channel;

            this.Series.Tags.Clear();

            foreach (var tag in this.TagsSource.Items)
            {
                this.Series.Tags.Add(tag);
            }

            return this.Series;
        }
    }
}
