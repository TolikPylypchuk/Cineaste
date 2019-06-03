using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media.Imaging;

using MovieList.Data.Models;
using MovieList.Properties;

namespace MovieList.ViewModels.FormItems
{
    public class SeasonFormItem : SeriesComponentFormItemBase
    {
        private bool isWatched;
        private bool isReleased;
        private string channel;
        private ObservableCollection<PeriodFormItem> periods;
        private ObservableCollection<BitmapImage> posters;

        public SeasonFormItem(Season season)
        {
            this.Season = season;
            this.CopySeasonProperties();
            this.IsInitialized = true;
        }

        public Season Season { get; }

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

        [Required(
            ErrorMessageResourceName = nameof(Messages.ChannelRequired),
            ErrorMessageResourceType = typeof(Messages))]
        public string Channel
        {
            get => this.channel;
            set
            {
                this.channel = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<PeriodFormItem> Periods
        {
            get => this.periods;
            set
            {
                this.periods = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<BitmapImage> Posters
        {
            get => this.posters;
            set
            {
                this.posters = value;
                this.OnPropertyChanged();
            }
        }

        protected override IEnumerable<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Titles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.Season.Titles.Where(t => !t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.OriginalTitles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.Season.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.IsWatched, () => this.Season.IsWatched),
                (() => this.IsReleased, () => this.Season.IsReleased),
                (() => this.Channel, () => this.Season.Channel),
                (() => this.Periods.Select(p => p.Period).OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth),
                 () => this.Season.Periods.OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth))
            };

        public override void WriteChanges()
        {
            if (this.Season.Id == default)
            {
                this.Season.Titles.Clear();
                this.Season.Periods.Clear();
            }

            foreach (var title in this.Titles.Union(this.OriginalTitles))
            {
                title.WriteChanges();

                if (title.Title.Id == default)
                {
                    this.Season.Titles.Add(title.Title);
                }
            }

            foreach (var period in this.Periods)
            {
                period.WriteChanges();

                if (period.Period.Id == default)
                {
                    this.Season.Periods.Add(period.Period);
                }
            }

            this.SetPosters();

            this.Season.IsWatched = this.IsWatched;
            this.Season.IsReleased = this.IsReleased;
            this.Season.Channel = this.Channel;

            this.AreChangesPresent = false;
        }

        public override void RevertChanges()
        {
            this.CopySeasonProperties();
            this.AreChangesPresent = false;
        }

        public override void OpenForm(SidePanelViewModel sidePanel)
            => sidePanel.OpenSeason.ExecuteIfCan(this);

        private void CopySeasonProperties()
        {
            this.CopyTitles(this.Season.Titles);

            this.IsWatched = this.Season.IsWatched;
            this.isReleased = this.Season.IsReleased;
            this.Channel = this.Season.Channel;

            this.SetPosters();
        }

        private void SetPosters()
        {
            this.Posters = new ObservableCollection<BitmapImage>(this.Periods
                .Select(p => p.PosterUrl)
                .Where(url => url != null)
                .Select(posterUrl =>
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(posterUrl, UriKind.Absolute);
                    bitmap.EndInit();

                    return bitmap;
                }));
        }
    }
}
