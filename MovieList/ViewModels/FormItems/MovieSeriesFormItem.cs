using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

using HandyControl.Data;

using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Properties;
using MovieList.ViewModels.ListItems;

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

        public MovieSeriesFormItem(MovieSeries movieSeries, IEnumerable<KindViewModel> allKinds)
            : this(movieSeries, null, allKinds)
        { }

        public MovieSeriesFormItem(
            MovieSeries movieSeries,
            MovieSeriesFormItem? parentSeries,
            IEnumerable<KindViewModel> allKinds)
        {
            this.MovieSeries = movieSeries;
            this.ParentSeries = parentSeries;
            this.AllKinds = allKinds;

            this.CopyMovieSeriesProperties();
            this.IsInitialized = true;
        }

        public MovieSeries MovieSeries { get; }
        public MovieSeriesFormItem? ParentSeries { get; }

        public IEnumerable<KindViewModel> AllKinds { get; set; }

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

                if (this.Components != null)
                {
                    foreach (var component in this.Components)
                    {
                        component.ForceRefreshProperty(nameof(component.NumberToDisplay));
                    }
                }
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
                this.components.CollectionChanged += this.OnComponentsChanged;

                foreach (var component in this.components)
                {
                    component.PropertyChanged += (s, e1) => this.OnPropertyChanged(nameof(this.Components));
                }

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

        public bool CanBeLooselyConnected
            => this.Components.All(c => c.DisplayNumber != null);

        public override string NumberToDisplay
            => (this.ParentSeries?.IsLooselyConnected ?? false)
                ? $"({this.MovieSeries.OrdinalNumber})"
                : this.MovieSeries.DisplayNumber?.ToString() ?? "-";

        protected override IEnumerable<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Titles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.MovieSeries.Titles.Where(t => !t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.OriginalTitles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.MovieSeries.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.Components.OrderBy(c => c.OrdinalNumber), this.GetMovieSeriesComponents),
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

            foreach (var component in this.Components)
            {
                component.WriteChanges();
            }

            this.AreChangesPresent = false;
        }

        public override void RevertChanges()
        {
            this.CopyMovieSeriesProperties();
            this.AreChangesPresent = false;
        }

        public override ListItem ToListItem(Configuration config)
            => new MovieSeriesListItem(this.MovieSeries, config);

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(this.Components))
            {
                base.OnPropertyChanged(nameof(this.CanBeLooselyConnected));
            }
        }

        private void OnComponentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (MovieSeriesComponentFormItemBase? component in e.NewItems)
            {
                if (component != null)
                {
                    component.PropertyChanged += (s, e1) => this.OnPropertyChanged(nameof(this.Components));
                }
            }

            this.OnPropertyChanged(nameof(this.Components));
        }

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
                this.GetMovieSeriesComponents());

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

        private IEnumerable<MovieSeriesComponentFormItemBase> GetMovieSeriesComponents()
            => this.MovieSeries.Entries
                    .Select(this.CreateFormItem)
                    .Union(this.MovieSeries.Parts.Select(p => new MovieSeriesFormItem(p, this, this.AllKinds)))
                    .OrderBy(item => item.OrdinalNumber);

        private MovieSeriesComponentFormItemBase CreateFormItem(MovieSeriesEntry entry)
            => entry.Movie != null
                ? (MovieSeriesComponentFormItemBase)new MovieFormItem(entry.Movie, this, this.AllKinds)
                : new SeriesFormItem(entry.Series!, this, this.AllKinds);
    }
}
