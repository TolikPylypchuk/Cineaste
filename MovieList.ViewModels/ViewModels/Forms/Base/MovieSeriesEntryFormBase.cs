using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
                .Select(e => e.SequenceNumber)
                .Max()
                ?? 0;

            var canGoToMiniseries = this.IfMovieSeriesPresent(() => formNotChanged);

            var canGoToNext = this.IfMovieSeriesPresent(() =>
                this.MovieSeriesEntry!.SequenceNumber == lastSequenceNumber
                    ? Observable.Return(false)
                    : formNotChanged);

            var canGoToPrevious = this.IfMovieSeriesPresent(() =>
                this.MovieSeriesEntry!.SequenceNumber == 1
                    ? Observable.Return(false)
                    : formNotChanged);

            this.GoToMovieSeries = ReactiveCommand.Create<Unit, MovieSeries>(
                _ => this.MovieSeriesEntry!.ParentSeries, canGoToMiniseries);

            this.GoToNext = ReactiveCommand.Create<Unit, MovieSeriesEntry>(
                _ => this.MovieSeriesEntry!.ParentSeries.Entries
                    .First(e => e.SequenceNumber > this.MovieSeriesEntry!.SequenceNumber),
                canGoToNext);

            this.GoToPrevious = ReactiveCommand.Create<Unit, MovieSeriesEntry>(
                _ => this.MovieSeriesEntry!.ParentSeries.Entries
                    .Last(e => e.SequenceNumber < this.MovieSeriesEntry!.SequenceNumber),
                canGoToPrevious);
        }

        public MovieSeriesEntry? MovieSeriesEntry { get; }

        public ReactiveCommand<Unit, MovieSeries> GoToMovieSeries { get; }
        public ReactiveCommand<Unit, MovieSeriesEntry> GoToNext { get; }
        public ReactiveCommand<Unit, MovieSeriesEntry> GoToPrevious { get; }

        private IObservable<bool> IfMovieSeriesPresent(Func<IObservable<bool>> observableProvider)
            => this.MovieSeriesEntry == null
                ? Observable.Return(false)
                : observableProvider();
    }
}
