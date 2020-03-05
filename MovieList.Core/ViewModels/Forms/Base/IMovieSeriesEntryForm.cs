using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.ViewModels.Forms.Base
{
    public interface IMovieSeriesEntryForm : IReactiveObject
    {
        MovieSeriesEntry? MovieSeriesEntry { get; }
    }
}
