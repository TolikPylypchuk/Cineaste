using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using MovieList.Commands;
using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public class MovieSeriesFormItem : MovieSeriesComponentFormItemBase
    {
        private bool hasName;
        private bool isLooselyConnected;
        private ObservableCollection<MovieSeriesComponentFormItemBase> components;

        public MovieSeriesFormItem(MovieSeries movieSeries)
        {
            this.MovieSeries = movieSeries;

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
                        this.AddTitle.ExecuteIfCan(null);
                        this.AddOriginalTitle.ExecuteIfCan(null);
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

        private void CopyMovieSeriesProperties()
        {
            this.CopyTitles(this.MovieSeries.Titles);

            this.HasName = this.MovieSeries.Titles.Count != 0;
            this.IsLooselyConnected = this.MovieSeries.IsLooselyConnected;

            this.Components = new ObservableCollection<MovieSeriesComponentFormItemBase>();
        }
    }
}
