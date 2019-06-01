using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public class SpecialEpisodeFormItem : SeriesComponentFormItemBase
    {
        private int month;
        private string year;
        private bool isWatched;
        private bool isReleased;
        private string channel;
        private string? posterUrl;

        private BitmapImage? poster;

        public SpecialEpisodeFormItem(SpecialEpisode specialEpisode)
        {
            this.SpecialEpisode = specialEpisode;
            this.CopySpecialEpisodeProperties();
            this.IsInitialized = true;
        }

        public SpecialEpisode SpecialEpisode { get; }

        public int Month
        {
            get => this.month;
            set
            {
                this.month = value;
                this.OnPropertyChanged();
            }
        }

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

        public string Channel
        {
            get => this.channel;
            set
            {
                this.channel = value;
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
                 () => this.SpecialEpisode.Titles.Where(t => !t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.OriginalTitles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.SpecialEpisode.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.Month, () => this.SpecialEpisode.Month),
                (() => this.Year, () => this.SpecialEpisode.Year),
                (() => this.IsWatched, () => this.SpecialEpisode.IsWatched),
                (() => this.IsReleased, () => this.SpecialEpisode.IsReleased),
                (() => this.Channel, () => this.SpecialEpisode.Channel),
                (() => this.PosterUrl, () => this.SpecialEpisode.PosterUrl),
            };

        public override void WriteChanges()
        {
            throw new NotImplementedException();
        }

        public override void RevertChanges()
        {
            this.CopySpecialEpisodeProperties();
            this.AreChangesPresent = false;
        }

        public override void OpenForm(SidePanelViewModel sidePanel)
            => sidePanel.OpenSpecialEpisode.ExecuteIfCan(this);

        private void CopySpecialEpisodeProperties()
        {
            this.CopyTitles(this.SpecialEpisode.Titles);

            this.Month = this.SpecialEpisode.Month;
            this.Year = this.SpecialEpisode.Year.ToString();
            this.IsWatched = this.SpecialEpisode.IsWatched;
            this.IsReleased = this.SpecialEpisode.IsReleased;
            this.Channel = this.SpecialEpisode.Channel;
            this.PosterUrl = this.SpecialEpisode.PosterUrl;

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
