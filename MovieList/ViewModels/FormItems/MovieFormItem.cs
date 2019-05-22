using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public class MovieFormItem : FormItemBase
    {
        private ObservableCollection<TitleFormItem> titles;
        private ObservableCollection<TitleFormItem> originalTitles;

        private int year;
        private bool isWatched;
        private bool isReleased;
        private string? imdbLink;
        private string? posterUrl;
        private KindViewModel kind;
        private BitmapImage? poster;

        public MovieFormItem(Movie movie, IEnumerable<KindViewModel> allKinds)
        {
            this.Movie = movie;
            this.AllKinds = allKinds;

            this.CopyMovieProperties();

            this.AddTitle = new DelegateCommand(
                _ => this.OnAddTitle(), _ => this.CanAddTitle());

            this.AddOriginalTitle = new DelegateCommand(
                _ => this.OnAddOriginalTitle(), _ => this.CanAddOriginalTitle());

            this.RemoveTitle = new DelegateCommand(
                this.OnRemoveTitle, _ => this.CanRemoveTitle());

            this.RemoveOriginalTitle = new DelegateCommand(
                this.OnRemoveOriginalTitle, _ => this.CanRemoveOriginalTitle());

            this.IsInitialized = true;
        }

        public ICommand AddTitle { get; }
        public ICommand RemoveTitle { get; }
        public ICommand AddOriginalTitle { get; }
        public ICommand RemoveOriginalTitle { get; }

        public Movie Movie { get; }
        public IEnumerable<KindViewModel> AllKinds { get; }

        public ObservableCollection<TitleFormItem> Titles
        {
            get => this.titles;
            set
            {
                this.titles = value;
                this.titles.CollectionChanged += this.OnTitlesChanged;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<TitleFormItem> OriginalTitles
        {
            get => this.originalTitles;
            set
            {
                this.originalTitles = value;
                this.originalTitles.CollectionChanged += this.OnTitlesChanged;
                this.OnPropertyChanged();
            }
        }

        public int Year
        {
            get => this.year;
            set
            {
                this.year = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsWatched
        {
            get => this.isWatched;
            set
            {
                this.isWatched = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsReleased
        {
            get => this.isReleased;
            set
            {
                this.isReleased = value;
                this.OnPropertyChanged();
            }
        }

        public string? ImdbLink
        {
            get => this.imdbLink;
            set
            {
                this.imdbLink = value;
                this.OnPropertyChanged();
            }
        }

        public string? PosterUrl
        {
            get => this.posterUrl;
            set
            {
                this.posterUrl = value;
                this.OnPropertyChanged();
            }
        }

        public KindViewModel Kind
        {
            get => this.kind;
            set
            {
                this.kind = value;
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

        protected override IEnumerable<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Titles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.Movie.Titles.Where(t => !t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.OriginalTitles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.Movie.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.Year, () => this.Movie.Year),
                (() => this.IsWatched, () => this.Movie.IsWatched),
                (() => this.IsReleased, () => this.Movie.IsReleased),
                (() => this.ImdbLink, () => this.Movie.ImdbLink),
                (() => this.PosterUrl, () => this.Movie.PosterUrl),
                (() => this.Kind.Kind.Id, () => this.Movie.KindId)
            };

        public override void WriteChanges()
        {
            foreach (var title in this.Titles.Union(this.OriginalTitles))
            {
                title.WriteChanges();
                if (title.Title.Id == default)
                {
                    this.Movie.Titles.Add(title.Title);
                } else
                {
                    this.Movie.Titles.First(t => t.Id == title.Title.Id);
                }
            }

            if (this.Movie.PosterUrl != this.PosterUrl)
            {
                this.SetPoster();
            }

            this.Movie.Year = this.Year;
            this.Movie.IsWatched = this.IsWatched;
            this.Movie.IsReleased = this.IsReleased;
            this.Movie.ImdbLink = this.ImdbLink;
            this.Movie.PosterUrl = this.PosterUrl;
            this.Movie.Kind = this.Kind.Kind;

            this.AreChangesPresent = false;
        }

        public override void RevertChanges()
        {
            this.CopyMovieProperties();
            this.AreChangesPresent = false;
        }

        private void CopyMovieProperties()
        {
            this.Titles = new ObservableCollection<TitleFormItem>(
                from title in this.Movie.Titles
                where !title.IsOriginal
                select new TitleFormItem(title));

            this.Titles.CollectionChanged += this.OnTitlesChanged;

            this.OriginalTitles = new ObservableCollection<TitleFormItem>(
                from title in this.Movie.Titles
                where title.IsOriginal
                select new TitleFormItem(title));

            this.OriginalTitles.CollectionChanged += this.OnTitlesChanged;

            foreach (var title in this.Titles)
            {
                title.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Titles));
            }

            foreach (var title in this.OriginalTitles)
            {
                title.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.OriginalTitles));
            }

            this.Year = this.Movie.Year;
            this.IsWatched = this.Movie.IsWatched;
            this.IsReleased = this.Movie.IsReleased;
            this.ImdbLink = this.Movie.ImdbLink;
            this.PosterUrl = this.Movie.PosterUrl;
            this.Kind = this.AllKinds.FirstOrDefault(k => k.Kind.Id == this.Movie.KindId) ?? this.AllKinds.First();

            this.SetPoster();
        }

        private void SetPoster()
        {
            if (this.PosterUrl != null)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(this.posterUrl, UriKind.Absolute);
                bitmap.EndInit();

                this.Poster = bitmap;
            }
        }

        private void OnAddTitle()
        {
            this.Titles.Add(new TitleFormItem(
                new Title { IsOriginal = false, Priority = this.Titles.Count }));
            this.OnPropertyChanged(nameof(this.Titles));
        }

        private bool CanAddTitle()
            => this.Titles.Count < 10;

        private void OnAddOriginalTitle()
        {
            this.OriginalTitles.Add(new TitleFormItem(
                new Title { IsOriginal = true, Priority = this.OriginalTitles.Count }));
            this.OnPropertyChanged(nameof(this.OriginalTitles));
        }

        private bool CanAddOriginalTitle()
            => this.OriginalTitles.Count < 10;

        private void OnRemoveTitle(object obj)
        {
            if (obj is TitleFormItem title)
            {
                this.Titles.Remove(title);

                foreach (var t in this.Titles.Where(t => t.Priority > title.Priority))
                {
                    t.Priority--;
                }
            }
        }

        private bool CanRemoveTitle()
            => this.Titles.Count != 1;

        private void OnRemoveOriginalTitle(object obj)
        {
            if (obj is TitleFormItem title)
            {
                this.OriginalTitles.Remove(title);

                foreach (var t in this.OriginalTitles.Where(t => t.Priority > title.Priority))
                {
                    t.Priority--;
                }
            }
        }

        private bool CanRemoveOriginalTitle()
            => this.OriginalTitles.Count != 1;

        private void OnTitlesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var title in e.NewItems.OfType<TitleFormItem>())
                {
                    title.PropertyChanged += (s, e) => this.OnPropertyChanged(
                        sender == this.titles ? nameof(this.Titles) : nameof(this.OriginalTitles));
                }
            }

            this.OnPropertyChanged(sender == this.titles ? nameof(this.Titles) : nameof(this.OriginalTitles));
        }
    }
}
