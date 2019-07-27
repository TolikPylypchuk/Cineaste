using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public abstract class MovieSeriesEntryFormItemBase : MovieSeriesComponentFormItemBase
    {
        private int ordinalNumber;
        private int? displayNumber;
        private bool shouldDisplayNumber;

        public MovieSeriesEntryFormItemBase(MovieSeriesEntry? movieSeriesEntry)
        {
            this.MovieSeriesEntry = movieSeriesEntry;
            this.CopyMovieSeriesEntryProperties();
        }

        public MovieSeriesEntry? MovieSeriesEntry { get; }

        public int OrdnialNumber
        {
            get => this.ordinalNumber;
            set
            {
                this.ordinalNumber = value;
                this.OnPropertyChanged();
            }
        }

        public int? DisplayNumber
        {
            get => this.displayNumber;
            set
            {
                this.displayNumber = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShouldDisplayNumber
        {
            get => this.shouldDisplayNumber;
            set
            {
                this.shouldDisplayNumber = value;
                this.OnPropertyChanged();
            }
        }

        protected void CopyMovieSeriesEntryProperties()
        {
            this.OrdnialNumber = this.MovieSeriesEntry?.OrdinalNumber ?? 1;
            this.DisplayNumber = this.MovieSeriesEntry != null ? this.MovieSeriesEntry.DisplayNumber : 1;
            this.ShouldDisplayNumber = this.DisplayNumber != null;
        }
    }
}
