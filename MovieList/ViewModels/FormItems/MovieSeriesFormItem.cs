using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public class MovieSeriesFormItem : MovieSeriesComponentFormItemBase
    {
        private bool hasName;
        private bool isLooselyConnected;
        private ObservableCollection<MovieSeriesComponentFormItemBase> components;

        private readonly IEnumerable<KindViewModel> allKinds;

        public MovieSeriesFormItem(MovieSeries movieSeries, IEnumerable<KindViewModel> allKinds)
        {
            this.MovieSeries = movieSeries;
            this.allKinds = allKinds;

            this.CopyMovieSeriesProperties();
            this.IsInitialized = true;
        }

        public MovieSeries MovieSeries { get; }

        public bool HasName
        {
            get => this.hasName;
            set
            {
                this.hasName = value;

                if (value)
                {
                    if (this.Titles.Count == 0)
                    {
                        this.AddTitle.ExecuteIfCan();
                        this.AddOriginalTitle.ExecuteIfCan();
                    }
                } else
                {
                    this.Titles.Clear();
                    this.OriginalTitles.Clear();
                }

                this.OnPropertyChanged();
            }
        }

        public bool IsLooselyConnected
        {
            get => this.isLooselyConnected;
            set
            {
                this.isLooselyConnected = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<MovieSeriesComponentFormItemBase> Components
        {
            get => this.components;
            set
            {
                this.components = value;
                this.components.CollectionChanged +=
                    (sender, e) => this.OnPropertyChanged(nameof(this.Components));
                this.OnPropertyChanged();
            }
        }

        public override string Title
            => this.MovieSeries.GetTitleName();

        public override string Years
        {
            get
            {
                var startYear = this.MovieSeries.GetStartYear();
                var endYear = this.MovieSeries.GetEndYear();

                return startYear == endYear ? startYear.ToString() : $"{startYear}-{endYear}";
            }
        }

        public override string NumberToDisplay
            => this.MovieSeries.OrdinalNumber?.ToString() ?? String.Empty;

        protected override IEnumerable<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Titles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.MovieSeries.Titles.Where(t => !t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.OriginalTitles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.MovieSeries.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.IsLooselyConnected, () => this.MovieSeries.IsLooselyConnected)
            };

        public override void WriteChanges()
        {
            if (this.MovieSeries.Id == default)
            {
                this.MovieSeries.Titles.Clear();
            }

            foreach (var title in this.Titles.Union(this.OriginalTitles))
            {
                title.WriteChanges();

                if (title.Title.Id == default)
                {
                    this.MovieSeries.Titles.Add(title.Title);
                }
            }

            foreach (var title in this.RemovedTitles)
            {
                this.MovieSeries.Titles.Remove(title.Title);
            }

            this.RemovedTitles.Clear();

            this.MovieSeries.IsLooselyConnected = this.IsLooselyConnected;

            this.AreChangesPresent = false;
        }

        public override void RevertChanges()
        {
            this.CopyMovieSeriesProperties();
            this.AreChangesPresent = false;
        }

        public override void OpenForm(SidePanelViewModel sidePanel)
            => sidePanel.OpenMovieSeries.ExecuteIfCan(this.MovieSeries);

        private void CopyMovieSeriesProperties()
        {
            this.CopyTitles(this.MovieSeries.Titles);

            this.HasName = this.MovieSeries.Titles.Count != 0;
            this.IsLooselyConnected = this.MovieSeries.IsLooselyConnected;

            this.Components = new ObservableCollection<MovieSeriesComponentFormItemBase>(
                this.MovieSeries.Entries
                    .Select(e => e.Movie != null
                        ? (MovieSeriesComponentFormItemBase)new MovieFormItem(e.Movie, this.allKinds)
                        : new SeriesFormItem(e.Series!, this.allKinds))
                    .Union(this.MovieSeries.Parts.Select(p => new MovieSeriesFormItem(p, this.allKinds)))
                    .OrderBy(i => i.OrdinalNumber));
        }
    }
}
