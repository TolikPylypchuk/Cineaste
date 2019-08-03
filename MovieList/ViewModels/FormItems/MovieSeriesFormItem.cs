using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media.Imaging;

using HandyControl.Data;

using MovieList.Data.Models;
using MovieList.Properties;

namespace MovieList.ViewModels.FormItems
{
    public class MovieSeriesFormItem : MovieSeriesComponentFormItemBase
    {
        private bool hasName;
        private bool isLooselyConnected;
        private string? posterUrl;
        private BitmapImage? poster;

        private ObservableCollection<MovieSeriesComponentFormItemBase> components;
        private ObservableCollection<MovieSeriesComponentFormItemBase> detachedComponents =
            new ObservableCollection<MovieSeriesComponentFormItemBase>();

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

        [Url(
            ErrorMessageResourceName = nameof(Messages.InvalidPosterUrl),
            ErrorMessageResourceType = typeof(Messages))]
        public string? PosterUrl
        {
            get => this.posterUrl;
            set
            {
                this.posterUrl = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage? Poster
        {
            get => this.poster;
            set
            {
                this.poster = value;
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

        public ObservableCollection<MovieSeriesComponentFormItemBase> DetachedComponents
        {
            get => this.detachedComponents;
            set
            {
                this.detachedComponents = value;
                this.detachedComponents.CollectionChanged +=
                    (sender, e) => this.OnPropertyChanged(nameof(this.DetachedComponents));
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
                (() => this.IsLooselyConnected, () => this.MovieSeries.IsLooselyConnected),
                (() => this.PosterUrl, () => this.MovieSeries.PosterUrl),
            };

        public Func<string, OperationResult<bool>> VerifyPosterUrl
            => this.Verify(nameof(this.PosterUrl));

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

            if (this.MovieSeries.PosterUrl != this.PosterUrl)
            {
                this.SetPoster();
            }

            this.MovieSeries.OrdinalNumber = this.OrdinalNumber;
            this.MovieSeries.DisplayNumber = this.DisplayNumber;
            this.MovieSeries.IsLooselyConnected = this.IsLooselyConnected;
            this.MovieSeries.PosterUrl = this.PosterUrl;

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

            this.OrdinalNumber = this.MovieSeries.OrdinalNumber ?? 0;
            this.DisplayNumber = this.MovieSeries.DisplayNumber;
            this.ShouldDisplayNumber = this.MovieSeries.DisplayNumber != null;
            this.HasName = this.MovieSeries.Titles.Count != 0;
            this.IsLooselyConnected = this.MovieSeries.IsLooselyConnected;
            this.PosterUrl = this.MovieSeries.PosterUrl;

            this.Components = new ObservableCollection<MovieSeriesComponentFormItemBase>(
                this.MovieSeries.Entries
                    .Select(e => e.Movie != null
                        ? (MovieSeriesComponentFormItemBase)new MovieFormItem(e.Movie, this.allKinds)
                        : new SeriesFormItem(e.Series!, this.allKinds))
                    .Union(this.MovieSeries.Parts.Select(p => new MovieSeriesFormItem(p, this.allKinds)))
                    .OrderBy(i => i.OrdinalNumber));

            this.SetPoster();
        }

        private void SetPoster()
        {
            if (this.PosterUrl != null)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(this.PosterUrl, UriKind.Absolute);
                bitmap.EndInit();

                this.Poster = bitmap;
            }
        }
    }
}
