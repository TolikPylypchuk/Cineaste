using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public sealed class MovieSeriesFormViewModel : MovieSeriesComponentFormBase<MovieSeries, MovieSeriesFormViewModel>
    {
        private readonly IEntityService<MovieSeries> movieSeriesService;

        public MovieSeriesFormViewModel(
            MovieSeries movieSeries,
            string fileName,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IEntityService<MovieSeries>? movieSeriesService = null)
            : base(movieSeries.Entry, resourceManager, scheduler)
        {
            this.MovieSeries = movieSeries;

            this.movieSeriesService = movieSeriesService ??
                Locator.Current.GetService<IEntityService<MovieSeries>>(fileName);

            this.PosterUrlRule = this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

            var formTitleWhenHasTitles = this.FormTitle;
            var formTitleWhenDoesNotHaveTitles = Observable.Return(this.GetFormTitle(this.MovieSeries));

            this.FormTitle =
                this.WhenAnyValue(vm => vm.HasTitles)
                    .Select(hasTitles => hasTitles ? formTitleWhenHasTitles : formTitleWhenDoesNotHaveTitles)
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler);

            this.WhenAnyValue(vm => vm.HasTitles)
                .BindTo(this, vm => vm.ShowTitles);

            this.WhenAnyValue(vm => vm.HasTitles)
                .Where(hasTitles => hasTitles && this.Titles.Count == 0)
                .Discard()
                .SubscribeAsync(this.AddTitles);

            this.CopyProperties();
            this.EnableChangeTracking();
        }

        public MovieSeries MovieSeries { get; }

        [Reactive]
        public bool HasTitles { get; set; }

        [Reactive]
        public bool ShowTitles { get; set; }

        [Reactive]
        public bool IsLooselyConnected { get; set; }

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        public ValidationHelper PosterUrlRule { get; }

        public override bool IsNew
            => this.MovieSeries.Id == default;

        protected override MovieSeriesFormViewModel Self
            => this;

        protected override ICollection<Title> ItemTitles
            => this.MovieSeries.Titles;

        protected override string NewItemKey
            => "NewMovieSeries";

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.HasTitles, vm => vm.MovieSeries.Titles.Count > 0);
            this.TrackChanges(vm => vm.ShowTitles, vm => vm.MovieSeries.ShowTitles);
            this.TrackChanges(vm => vm.IsLooselyConnected, vm => vm.MovieSeries.IsLooselyConnected);
            this.TrackChanges(vm => vm.PosterUrl, vm => vm.MovieSeries.PosterUrl.EmptyIfNull());

            base.EnableChangeTracking();
        }

        protected override async Task<MovieSeries> OnSaveAsync()
        {
            if (!this.HasTitles)
            {
                this.ClearTitles();
            }

            await this.SaveTitlesAsync();

            this.MovieSeries.ShowTitles = this.ShowTitles;
            this.MovieSeries.IsLooselyConnected = this.IsLooselyConnected;
            this.MovieSeries.PosterUrl = this.PosterUrl.NullIfEmpty();

            await this.movieSeriesService.SaveAsync(this.MovieSeries);

            return this.MovieSeries;
        }

        protected override Task<MovieSeries?> OnDeleteAsync()
            => throw new NotSupportedException("A movie series cannot be deleted directly.");

        protected override void CopyProperties()
        {
            base.CopyProperties();

            this.HasTitles = this.MovieSeries.Titles.Count > 0;
            this.ShowTitles = this.MovieSeries.ShowTitles;
            this.IsLooselyConnected = this.MovieSeries.IsLooselyConnected;
            this.PosterUrl = this.MovieSeries.PosterUrl.EmptyIfNull();
        }

        protected override void AttachTitle(Title title)
            => title.MovieSeries = this.MovieSeries;

        private string GetFormTitle(MovieSeries movieSeries)
        {
            string title = movieSeries.ActualTitles.First(t => !t.IsOriginal).Name;
            return movieSeries.Entry == null ? title : $"{this.GetFormTitle(movieSeries.Entry.ParentSeries)}: {title}";
        }

        private async Task AddTitles()
        {
            string titleName = this.MovieSeries.ActualTitles.First(t => !t.IsOriginal).Name;
            string originalTitleName = this.MovieSeries.ActualTitles.First(t => t.IsOriginal).Name;

            await this.AddTitle.Execute();
            await this.AddOriginalTitle.Execute();

            this.Titles[0].Name = titleName;
            this.OriginalTitles[0].Name = originalTitleName;
        }
    }
}
