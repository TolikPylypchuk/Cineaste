using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        private Kind kind;

        public MovieFormItem(Movie movie)
        {
            this.Movie = movie;

            this.titles = new ObservableCollection<TitleFormItem>(
                from title in movie.Titles
                where !title.IsOriginal
                select new TitleFormItem(title));

            this.originalTitles = new ObservableCollection<TitleFormItem>(
                from title in movie.Titles
                where title.IsOriginal
                select new TitleFormItem(title));

            this.year = movie.Year;
            this.isWatched = movie.IsWatched;
            this.isReleased = movie.IsReleased;
            this.imdbLink = movie.ImdbLink;
            this.posterUrl = movie.PosterUrl;
            this.kind = movie.Kind;
        }

        public Movie Movie { get; }

        public ObservableCollection<TitleFormItem> Titles
        {
            get => this.titles;
            set
            {
                this.titles = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<TitleFormItem> OriginalTitles
        {
            get => this.originalTitles;
            set
            {
                this.originalTitles = value;
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

        public Kind Kind
        {
            get => this.kind;
            set
            {
                this.kind = value;
                this.OnPropertyChanged();
            }
        }

        protected override List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Titles.OrderBy(t => t.Name),
                 () => this.Movie.Titles.Where(t => !t.IsOriginal).OrderBy(t => t.Name)),
                (() => this.OriginalTitles.OrderBy(t => t.Name),
                 () => this.Movie.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Name)),
                (() => this.Year, () => this.Movie.Year),
                (() => this.IsWatched, () => this.Movie.IsWatched),
                (() => this.IsReleased, () => this.Movie.IsReleased),
                (() => this.ImdbLink, () => this.Movie.ImdbLink),
                (() => this.Movie.PosterUrl, () => this.Movie.PosterUrl),
                (() => this.Kind.Id, () => this.Movie.Kind.Id)
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

            this.Movie.Year = this.Year;
            this.Movie.IsWatched = this.IsWatched;
            this.Movie.IsReleased = this.IsReleased;
            this.Movie.ImdbLink = this.ImdbLink;
            this.Movie.PosterUrl = this.PosterUrl;
            this.Movie.Kind = this.Kind;
        }
    }
}
