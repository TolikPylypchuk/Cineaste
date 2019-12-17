using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.DialogModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeriesFormViewModel : TitledFormViewModelBase<Series, SeriesFormViewModel>
    {
        private readonly IEntityService<Series> seriesService;

        private readonly SourceList<Season> seasonsSource = new SourceList<Season>();

        private readonly ReadOnlyObservableCollection<SeasonFormViewModel> seasons;
        private readonly ReadOnlyObservableCollection<SeriesComponentViewModel> components;

        private readonly BehaviorSubject<int> maxSequenceNumberSubject = new BehaviorSubject<int>(0);
        private readonly Subject<Unit> resort = new Subject<Unit>();

        public SeriesFormViewModel(
            Series series,
            ReadOnlyObservableCollection<Kind> kinds,
            string fileName,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IEntityService<Series>? seriesService = null)
            : base(resourceManager, scheduler)
        {
            this.Series = series;
            this.Kinds = kinds;

            this.seriesService = seriesService ?? Locator.Current.GetService<IEntityService<Series>>(fileName);

            this.CopyProperties();

            this.seasonsSource.Connect()
                .Transform(this.CreateSeasonForm)
                .Sort(
                    SortExpressionComparer<SeasonFormViewModel>.Ascending(season => season.SequenceNumber),
                    resort: this.resort)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.seasons)
                .DisposeMany()
                .Subscribe();

            this.seasons.ToObservableChangeSet()
                .Transform(this.CreateComponent)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.components)
                .DisposeMany()
                .Subscribe();

            this.Components.ToObservableChangeSet()
                .AutoRefresh(component => component.SequenceNumber)
                .Select(_ => this.Components.Count != 0
                    ? this.Components.Max(component => component.SequenceNumber)
                    : 0)
                .Subscribe(this.maxSequenceNumberSubject);

            this.ImdbLinkRule = this.ValidationRule(vm => vm.ImdbLink, link => link.IsUrl(), "ImdbLinkInvalid");
            this.PosterUrlRule = this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

            this.CanDeleteWhenNotNew();

            this.AddSeason = ReactiveCommand.Create(this.OnAddSeason);
            this.SelectComponent = ReactiveCommand.Create<ReactiveObject, ReactiveObject>(form => form);

            this.EnableChangeTracking();
        }

        public Series Series { get; }

        public ReadOnlyObservableCollection<Kind> Kinds { get; }

        [Reactive]
        public Kind Kind { get; set; } = null!;

        [Reactive]
        public bool IsAnthology { get; set; }

        [Reactive]
        public SeriesWatchStatus WatchStatus { get; set; }

        [Reactive]
        public SeriesReleaseStatus ReleaseStatus { get; set; }

        public ReadOnlyObservableCollection<SeasonFormViewModel> Seasons
            => this.seasons;

        public ReadOnlyObservableCollection<SeriesComponentViewModel> Components
            => this.components;

        [Reactive]
        public string ImdbLink { get; set; } = String.Empty;

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        public ValidationHelper ImdbLinkRule { get; }
        public ValidationHelper PosterUrlRule { get; }

        public ReactiveCommand<Unit, Unit> AddSeason { get; }
        public ReactiveCommand<ReactiveObject, ReactiveObject> SelectComponent { get; }

        public override bool IsNew
            => this.Series.Id == default;

        protected override SeriesFormViewModel Self
            => this;

        protected override ICollection<Title> ItemTitles
            => this.Series.Titles;

        protected override string NewItemKey
            => "NewSeries";

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.WatchStatus, vm => vm.Series.WatchStatus);
            this.TrackChanges(vm => vm.ReleaseStatus, vm => vm.Series.ReleaseStatus);
            this.TrackChanges(vm => vm.Kind, vm => vm.Series.Kind);
            this.TrackChanges(vm => vm.IsAnthology, vm => vm.Series.IsAnthology);
            this.TrackChanges(vm => vm.ImdbLink, vm => vm.Series.ImdbLink.EmptyIfNull());
            this.TrackChanges(vm => vm.PosterUrl, vm => vm.Series.PosterUrl.EmptyIfNull());
            this.TrackChanges(this.IsCollectionChanged(vm => vm.Seasons, vm => vm.Series.Seasons));

            this.TrackValidation(this.IsCollectionValid<SeasonFormViewModel, Season>(this.Seasons));

            base.EnableChangeTracking();
        }

        protected override async Task<Series> OnSaveAsync()
        {
            await this.SaveTitlesAsync();
            await this.SaveSeasonsAsync();

            this.Series.IsAnthology = this.IsAnthology;
            this.Series.WatchStatus = this.WatchStatus;
            this.Series.ReleaseStatus = this.ReleaseStatus;
            this.Series.Kind = this.Kind;
            this.Series.ImdbLink = this.ImdbLink.NullIfEmpty();
            this.Series.PosterUrl = this.PosterUrl.NullIfEmpty();

            await this.seriesService.SaveAsync(this.Series);

            return this.Series;
        }

        protected override async Task<Series?> OnDeleteAsync()
        {
            bool shouldDelete = await Dialog.Confirm.Handle(new ConfirmationModel("DeleteSeries"));

            if (shouldDelete)
            {
                await this.seriesService.DeleteAsync(this.Series);
                return this.Series;
            }

            return null;
        }

        protected override void CopyProperties()
        {
            base.CopyProperties();

            this.seasonsSource.Clear();
            this.seasonsSource.AddRange(this.Series.Seasons);

            this.IsAnthology = this.Series.IsAnthology;
            this.Kind = this.Series.Kind;
            this.WatchStatus = this.Series.WatchStatus;
            this.ReleaseStatus = this.Series.ReleaseStatus;
            this.ImdbLink = this.Series.ImdbLink.EmptyIfNull();
            this.PosterUrl = this.Series.PosterUrl.EmptyIfNull();
        }

        protected override void AttachTitle(Title title)
            => title.Series = this.Series;

        private void OnAddSeason()
        {
            var period = new Period
            {
                StartMonth = 1,
                StartYear = 2000,
                EndMonth = 1,
                EndYear = 2000
            };

            var season = new Season
            {
                Titles = new List<Title>
                {
                    new Title { Priority = 1, IsOriginal = false },
                    new Title { Priority = 1, IsOriginal = true }
                },
                Series = this.Series,
                Periods = new List<Period> { period },
                SequenceNumber = this.Seasons.Count + 1
            };

            period.Season = season;

            this.seasonsSource.Add(season);
        }

        private SeasonFormViewModel CreateSeasonForm(Season season)
        {
            var seasonForm = new SeasonFormViewModel(
                season, this, this.maxSequenceNumberSubject, this.ResourceManager, this.Scheduler);

            seasonForm.Delete
                .WhereNotNull()
                .Subscribe(s => this.seasonsSource.Remove(s));

            seasonForm.SelectNext
                .Select(_ => this.Components.First(component =>
                    component.SequenceNumber == seasonForm.SequenceNumber + 1))
                .Select(component => component.Form)
                .SubscribeAsync(async form => await this.SelectComponent.Execute(form));

            seasonForm.SelectPrevious
                .Select(_ => this.Components.First(component =>
                    component.SequenceNumber == seasonForm.SequenceNumber - 1))
                .Select(component => component.Form)
                .SubscribeAsync(async form => await this.SelectComponent.Execute(form));

            seasonForm.WhenAnyValue(form => form.SequenceNumber)
                .StartWith(seasonForm.SequenceNumber)
                .Buffer(2, 1)
                .Select(values => (Old: values[0], New: values[1]))
                .Where(values => values.Old != values.New)
                .Subscribe(values => this.Seasons
                    .Where(form => form != seasonForm && form.SequenceNumber == seasonForm.SequenceNumber)
                    .ForEach(form => form.SequenceNumber = values.New < values.Old
                        ? form.SequenceNumber + 1
                        : form.SequenceNumber - 1));

            seasonForm.WhenAnyValue(form => form.SequenceNumber)
                .Discard()
                .Subscribe(this.resort);

            return seasonForm;
        }

        private SeriesComponentViewModel CreateComponent(SeasonFormViewModel season)
        {
            var component = new SeriesComponentViewModel(season);

            component.Select.InvokeCommand(this.SelectComponent);

            return component;
        }

        private async Task SaveSeasonsAsync()
        {
            foreach (var season in this.Seasons)
            {
                await season.Save.Execute();
            }

            foreach (var season in this.seasonsSource.Items.Except(this.Series.Seasons).ToList())
            {
                this.Series.Seasons.Add(season);
            }

            foreach (var season in this.Series.Seasons.Except(this.seasonsSource.Items).ToList())
            {
                this.Series.Seasons.Remove(season);
            }
        }

    }
}
