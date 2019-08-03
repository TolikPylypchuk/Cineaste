using System;

using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public abstract class MovieSeriesEntryFormItemBase : MovieSeriesComponentFormItemBase
    {
        public MovieSeriesEntryFormItemBase(MovieSeriesEntry? movieSeriesEntry)
        {
            this.MovieSeriesEntry = movieSeriesEntry;
            this.CopyMovieSeriesEntryProperties();
        }

        public MovieSeriesEntry? MovieSeriesEntry { get; }

        public override string NumberToDisplay
            => this.MovieSeriesEntry?.GetDisplayNumber() ?? String.Empty;

        protected void CopyMovieSeriesEntryProperties()
        {
            this.OrdinalNumber = this.MovieSeriesEntry?.OrdinalNumber ?? 1;
            this.DisplayNumber = this.MovieSeriesEntry != null ? this.MovieSeriesEntry.DisplayNumber : 1;
            this.ShouldDisplayNumber = this.DisplayNumber != null;
        }
    }
}
