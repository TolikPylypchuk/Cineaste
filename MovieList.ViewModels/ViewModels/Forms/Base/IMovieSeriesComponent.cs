using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.ViewModels.Forms.Base
{
    public interface IMovieSeriesComponent : IReactiveObject
    {
        MovieSeriesEntry? MovieSeriesEntry { get; }
    }
}
