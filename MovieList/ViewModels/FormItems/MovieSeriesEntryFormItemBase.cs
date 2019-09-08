using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public abstract class MovieSeriesEntryFormItemBase : MovieSeriesComponentFormItemBase
    {
        protected MovieSeriesEntryFormItemBase(MovieSeriesEntry? movieSeriesEntry, MovieSeriesFormItem? movieSeries)
        {
            this.MovieSeriesEntry = movieSeriesEntry;
            this.MovieSeries = movieSeries;
            this.CopyMovieSeriesEntryProperties();
        }

        public MovieSeriesEntry? MovieSeriesEntry { get; }
        public MovieSeriesFormItem? MovieSeries { get; }

        public override string NumberToDisplay
            => (this.MovieSeries?.IsLooselyConnected ?? false)
                ? $"({this.OrdinalNumber})"
                : this.DisplayNumber?.ToString() ?? "-";

        public override void RevertChanges()
            => this.CopyMovieSeriesEntryProperties();

        public override void WriteChanges()
        {
            if (this.MovieSeriesEntry != null)
            {
                this.MovieSeriesEntry.OrdinalNumber = this.OrdinalNumber;
                this.MovieSeriesEntry.DisplayNumber = this.DisplayNumber;
            }
        }

        protected void CopyMovieSeriesEntryProperties()
        {
            this.OrdinalNumber = this.MovieSeriesEntry?.OrdinalNumber ?? 1;
            this.DisplayNumber = this.MovieSeriesEntry != null ? this.MovieSeriesEntry.DisplayNumber : 1;
            this.ShouldDisplayNumber = this.DisplayNumber != null;
        }
    }
}
