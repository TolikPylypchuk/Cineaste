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

using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.DialogModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class MovieFormViewModel : TitledFormViewModelBase<Movie, MovieFormViewModel>
    {
        private readonly IMovieService movieService;

        public MovieFormViewModel(
            Movie movie,
            ReadOnlyObservableCollection<Kind> kinds,
            string fileName,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IMovieService? movieService = null)
            : base(resourceManager, scheduler)
        {
            this.Movie = movie;
            this.Kinds = kinds;

            this.movieService = movieService ?? Locator.Current.GetService<IMovieService>(fileName);

            this.CopyProperties();

            this.YearRule = this.CreateYearRule();
            this.ImdbLinkRule = this.CreateImdbLinkRule();
            this.PosterUrlRule = this.CreatePosterUrlRule();

            this.InitializeValueDependencies();

            Observable.Return(this.Movie.Id != default)
                .Merge(this.Save.Select(_ => true))
                .Subscribe(this.CanDeleteSubject);

            this.Close = ReactiveCommand.Create(() => { });

            this.InitializeChangeTracking();
        }

        public Movie Movie { get; }

        public ReadOnlyObservableCollection<Kind> Kinds { get; }

        [Reactive]
        public string Year { get; set; } = String.Empty;

        [Reactive]
        public Kind Kind { get; set; } = null!;

        [Reactive]
        public bool IsWatched { get; set; }

        [Reactive]
        public bool IsReleased { get; set; }

        [Reactive]
        public string ImdbLink { get; set; } = null!;

        [Reactive]
        public string PosterUrl { get; set; } = null!;

        public ValidationHelper YearRule { get; }
        public ValidationHelper ImdbLinkRule { get; }
        public ValidationHelper PosterUrlRule { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }

        public override bool IsNew
            => this.Movie.Id == default;

        protected override IEnumerable<Title> ItemTitles
            => this.Movie.Titles;

        protected override string NewItemKey
            => "NewMovie";

        protected override void InitializeChangeTracking()
        {
            var yearChanged = this.WhenAnyValue(vm => vm.Year)
                .Select(year => year != this.Movie.Year.ToString())
                .Do(changed => this.Log().Debug(changed ? "Year is changed" : "Year is unchanged"));

            var kindChanged = this.WhenAnyValue(vm => vm.Kind)
                .Select(kind => kind != this.Movie.Kind)
                .Do(changed => this.Log().Debug(changed ? "Kind is changed" : "Kind is unchanged"));

            var isWatchedChanged = this.WhenAnyValue(vm => vm.IsWatched)
                .Select(isWatched => isWatched != this.Movie.IsWatched)
                .Do(changed => this.Log().Debug(changed ? "Is watched is changed" : "Is watched is unchanged"));

            var isReleasedChanged = this.WhenAnyValue(vm => vm.IsReleased)
                .Select(isReleased => isReleased != this.Movie.IsReleased)
                .Do(changed => this.Log().Debug(changed ? "Is released is changed" : "Is released is unchanged"));

            var imdbLinkChanged = this.WhenAnyValue(vm => vm.ImdbLink)
                .Select(link => link.NullIfEmpty() != this.Movie.ImdbLink)
                .Do(changed => this.Log().Debug(changed ? "IMDb link is changed" : "IMDb link is unchanged"));

            var posterUrlChanged = this.WhenAnyValue(vm => vm.PosterUrl)
                .Select(url => url.NullIfEmpty() != this.Movie.PosterUrl)
                .Do(changed => this.Log().Debug(changed ? "Poster URL is changed" : "Poster URL is unchanged"));

            var falseWhenSave = this.Save.Select(_ => false);
            var falseWhenCancel = this.Cancel.Select(_ => false);

            Observable.CombineLatest(
                    this.TitlesChanged,
                    this.OriginalTitlesChanged,
                    yearChanged,
                    kindChanged,
                    isWatchedChanged,
                    isReleasedChanged,
                    imdbLinkChanged,
                    posterUrlChanged)
                .AnyTrue()
                .Merge(falseWhenSave)
                .Merge(falseWhenCancel)
                .Subscribe(this.FormChangedSubject);

            Observable.CombineLatest(
                    this.TitlesValid,
                    this.OriginalTitlesValid,
                    this.YearRule.Valid(),
                    this.ImdbLinkRule.Valid(),
                    this.PosterUrlRule.Valid())
                .AllTrue()
                .Subscribe(this.ValidSubject);
        }

        protected override async Task<Movie> OnSaveAsync()
        {
            foreach (var title in this.Titles.Union(this.OriginalTitles))
            {
                await title.Save.Execute();
            }

            this.Movie.Titles.Add(this.TitlesSource.Items.Except(this.Movie.Titles).ToList());
            this.Movie.Titles.Remove(this.Movie.Titles.Except(this.TitlesSource.Items).ToList());

            this.Movie.IsWatched = this.IsWatched;
            this.Movie.IsReleased = this.IsReleased;
            this.Movie.Year = Int32.Parse(this.Year);
            this.Movie.Kind = this.Kind;
            this.Movie.ImdbLink = this.ImdbLink.NullIfEmpty();
            this.Movie.PosterUrl = this.PosterUrl.NullIfEmpty();

            await this.movieService.SaveAsync(this.Movie);

            return this.Movie;
        }

        protected override async Task<Movie?> OnDeleteAsync()
        {
            bool shouldDelete = await Dialog.Confirm.Handle(new ConfirmationModel("DeleteMovie"));

            if (shouldDelete)
            {
                await this.movieService.DeleteAsync(this.Movie);
                return this.Movie;
            }

            return null;
        }

        protected override void CopyProperties()
        {
            this.TitlesSource.Clear();
            this.TitlesSource.AddRange(this.Movie.Titles);

            this.Year = this.Movie.Year.ToString();
            this.Kind = this.Movie.Kind;
            this.IsWatched = this.Movie.IsWatched;
            this.IsReleased = this.Movie.IsReleased;
            this.ImdbLink = this.Movie.ImdbLink.EmptyIfNull();
            this.PosterUrl = this.Movie.PosterUrl.EmptyIfNull();
        }

        protected override void AttachTitle(Title title)
            => title.Movie = this.Movie;

        private void InitializeValueDependencies()
        {
            this.WhenAnyValue(vm => vm.IsReleased)
                .Where(isReleased => !isReleased)
                .Subscribe(_ => this.IsWatched = false);

            this.WhenAnyValue(vm => vm.IsWatched)
                .Where(isWatched => isWatched)
                .Subscribe(_ => this.IsReleased = true);

            this.WhenAnyValue(vm => vm.Year)
                .Where(_ => this.YearRule.IsValid)
                .Select(Int32.Parse)
                .Where(year => year != this.Scheduler.Now.Year)
                .Subscribe(year => this.IsReleased = year < this.Scheduler.Now.Year);
        }

        private ValidationHelper CreateYearRule()
            => this.ValidationRule(
                vm => vm.Year,
                year => !String.IsNullOrWhiteSpace(year) &&
                        Int32.TryParse(year, out int value) &&
                        value >= MovieMinYear &&
                        value <= MovieMaxYear,
                year => String.IsNullOrWhiteSpace(year)
                    ? this.ResourceManager.GetString("ValidationYearEmpty")
                    : this.ResourceManager.GetString("ValidationYearInvalid"));

        private ValidationHelper CreateImdbLinkRule()
            => this.ValidationRule(
                vm => vm.ImdbLink,
                link => link.IsUrl(),
                this.ResourceManager.GetString("ValidationImdbLinkInvalid"));

        private ValidationHelper CreatePosterUrlRule()
            => this.ValidationRule(
                vm => vm.PosterUrl,
                url => url.IsUrl(),
                this.ResourceManager.GetString("ValidationPosterUrlInvalid"));
    }
}
