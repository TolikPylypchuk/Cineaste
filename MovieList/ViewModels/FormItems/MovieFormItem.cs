using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Windows.Media.Imaging;

using HandyControl.Data;

using MovieList.Data.Models;
using MovieList.Properties;
using MovieList.Validation;

namespace MovieList.ViewModels.FormItems
{
    public class MovieFormItem : TitledFormItemBase
    {
        private string year;
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
            this.IsInitialized = true;
        }

        public Movie Movie { get; }
        public IEnumerable<KindViewModel> AllKinds { get; }

        [Year(Min = 1850, Max = 2100)]
        [Required(ErrorMessageResourceName = "YearRequired", ErrorMessageResourceType = typeof(Messages))]
        public string Year
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

                if (this.IsWatched && !this.IsReleased)
                {
                    this.IsReleased = true;
                }
            }
        }

        public bool IsReleased
        {
            get => this.isReleased;
            set
            {
                this.isReleased = value;
                this.OnPropertyChanged();

                if (this.IsWatched && !this.IsReleased)
                {
                    this.IsWatched = false;
                }
            }
        }

        [Url(ErrorMessageResourceName = "InvalidImdbLink", ErrorMessageResourceType = typeof(Messages))]
        public string? ImdbLink
        {
            get => this.imdbLink;
            set
            {
                this.imdbLink = value;
                this.OnPropertyChanged();
            }
        }

        [Url(ErrorMessageResourceName = "InvalidPosterUrl", ErrorMessageResourceType = typeof(Messages))]
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

        public Func<string, OperationResult<bool>> VerifyYear
            => this.Verify(nameof(this.Year));

        public Func<string, OperationResult<bool>> VerifyImdbLink
            => this.Verify(nameof(this.ImdbLink));

        public Func<string, OperationResult<bool>> VerifyPosterUrl
            => this.Verify(nameof(this.PosterUrl));

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
            if (this.Movie.Id == default)
            {
                this.Movie.Titles.Clear();
            }

            foreach (var title in this.Titles.Union(this.OriginalTitles))
            {
                title.WriteChanges();

                if (title.Title.Id == default)
                {
                    this.Movie.Titles.Add(title.Title);
                }
            }

            if (this.Movie.PosterUrl != this.PosterUrl)
            {
                this.SetPoster();
            }

            this.Movie.Year = Int32.Parse(this.Year);
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
            this.CopyTitles(this.Movie.Titles);

            this.Year = this.Movie.Year.ToString();
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
    }
}
