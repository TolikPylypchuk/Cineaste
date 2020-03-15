using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.ViewModels.Forms.Base
{
    public abstract class MovieSeriesEntryFormBase<TModel, TViewModel>
        : TitledFormBase<TModel, TViewModel>, IMovieSeriesEntryForm
        where TModel : class
        where TViewModel : MovieSeriesEntryFormBase<TModel, TViewModel>
    {
        private readonly BehaviorSubject<bool> canCreateMovieSeriesSubject = new BehaviorSubject<bool>(false);

        protected MovieSeriesEntryFormBase(
            MovieSeriesEntry? entry,
            ResourceManager? resourceManager,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.MovieSeriesEntry = entry;

            var formNotChanged = this.FormChanged.Invert();

            int lastSequenceNumber = this.MovieSeriesEntry?.ParentSeries
                .Entries
                .Select(e => (int?)e.SequenceNumber)
                .Max()
                ?? 0;

            var canGoToMovieSeries = this.IfMovieSeriesPresent(() => formNotChanged);

            this.GoToMovieSeries = ReactiveCommand.Create<Unit, MovieSeries>(
                _ => this.MovieSeriesEntry!.ParentSeries, canGoToMovieSeries);

            var canGoToNext = this.IfMovieSeriesPresent(() =>
                this.MovieSeriesEntry!.SequenceNumber >= lastSequenceNumber
                    ? Observable.Return(false)
                    : formNotChanged);

            this.GoToNext = ReactiveCommand.Create<Unit, MovieSeriesEntry>(
                _ => this.MovieSeriesEntry!.ParentSeries.Entries
                    .OrderBy(e => e.SequenceNumber)
                    .First(e => e.SequenceNumber > this.MovieSeriesEntry!.SequenceNumber),
                canGoToNext);

            var canGoToPrevious = this.IfMovieSeriesPresent(() =>
                this.MovieSeriesEntry!.SequenceNumber == 1
                    ? Observable.Return(false)
                    : formNotChanged);

            this.GoToPrevious = ReactiveCommand.Create<Unit, MovieSeriesEntry>(
                _ => this.MovieSeriesEntry!.ParentSeries.Entries
                    .OrderBy(e => e.SequenceNumber)
                    .Last(e => e.SequenceNumber < this.MovieSeriesEntry!.SequenceNumber),
                canGoToPrevious);

            this.CreateMovieSeries = ReactiveCommand.Create(() => { }, this.canCreateMovieSeriesSubject);
        }

        public MovieSeriesEntry? MovieSeriesEntry { get; }

        public ReactiveCommand<Unit, MovieSeries> GoToMovieSeries { get; }
        public ReactiveCommand<Unit, MovieSeriesEntry> GoToNext { get; }
        public ReactiveCommand<Unit, MovieSeriesEntry> GoToPrevious { get; }
        public ReactiveCommand<Unit, Unit> CreateMovieSeries { get; }

        protected void CanCreateMovieSeries()
            => Observable.CombineLatest(
                   Observable.Return(!this.IsNew).Merge(this.Save.Select(_ => true)),
                   Observable.Return(this.MovieSeriesEntry == null),
                   this.FormChanged.Invert())
                .AllTrue()
                .Subscribe(this.canCreateMovieSeriesSubject);

        private IObservable<bool> IfMovieSeriesPresent(Func<IObservable<bool>> observableProvider)
            => this.MovieSeriesEntry == null
                ? Observable.Return(false)
                : observableProvider();
    }
}
