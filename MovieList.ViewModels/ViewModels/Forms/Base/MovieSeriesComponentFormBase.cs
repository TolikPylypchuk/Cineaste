using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.ViewModels.Forms.Base
{
    public abstract class MovieSeriesComponentFormBase<TModel, TViewModel>
        : TitledFormBase<TModel, TViewModel>, IMovieSeriesComponent
        where TModel : class
        where TViewModel : MovieSeriesComponentFormBase<TModel, TViewModel>
    {
        protected MovieSeriesComponentFormBase(
            MovieSeriesEntry? entry,
            ResourceManager? resourceManager,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.MovieSeriesEntry = entry;

            var canGoToMiniseries = this.MovieSeriesEntry == null
                ? Observable.Return(false)
                : this.Cancel.CanExecute.Invert();

            this.GoToMovieSeries = ReactiveCommand.Create<Unit, MovieSeries>(
                _ => this.MovieSeriesEntry!.ParentSeries, canGoToMiniseries);
        }

        public MovieSeriesEntry? MovieSeriesEntry { get; }

        public ReactiveCommand<Unit, MovieSeries> GoToMovieSeries { get; }
    }
}
