using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

using MovieList.Data.Models;

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
                 () => this.Season.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name))
            };

        public override void RevertChanges()
        {
            this.CopySeasonProperties();
            this.AreChangesPresent = false;
        }

        public override void WriteChanges()
        {
            throw new NotImplementedException();
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
