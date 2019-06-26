using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using HandyControl.Data;

using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public class SeasonFormItem : SeriesComponentFormItemBase
    {
        private bool isWatched;
        private bool isReleased;

        private ObservableCollection<PeriodFormItem> periods;

        private int posterIndex;
        private ObservableCollection<BitmapImage> posters;

        public SeasonFormItem(Season season)
        {
            this.Season = season;
            this.CopySeasonProperties();

            this.ShowNextPoster = new DelegateCommand(
                _ => this.PosterIndex++, _ => this.Posters.Count > 0 && this.PosterIndex != this.Posters.Count - 1);

            this.ShowPreviousPoster = new DelegateCommand(
                _ => this.PosterIndex--, _ => this.Posters.Count > 0 && this.PosterIndex != 0);

            this.AddPeriod = new DelegateCommand(_ => this.Periods.Add(this.NewPeriod(new Period())));
            this.RemovePeriod = new DelegateCommand(this.OnRemovePeriod, _ => this.CanRemovePeriod());

            this.IsInitialized = true;
        }

        public ICommand ShowNextPoster { get; }
        public ICommand ShowPreviousPoster { get; }

        public ICommand AddPeriod { get; }
        public ICommand RemovePeriod { get; }

        public Season Season { get; }

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

        public ObservableCollection<PeriodFormItem> Periods
        {
            get => this.periods;
            set
            {
                this.periods = value;
                this.periods.CollectionChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Periods));
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<BitmapImage> Posters
        {
            get => this.posters;
            set
            {
                this.posters = value;
                this.PosterIndex = 0;
                this.OnPropertyChanged();
            }
        }

        public int PosterIndex
        {
            get => this.posterIndex;
            set
            {
                this.posterIndex = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(Poster));
            }
        }

        public BitmapImage? Poster
            => this.Posters.Count != 0 ? this.Posters[this.posterIndex] : null;

        public override string Title
            => this.Season.Title.Name;

        public override string Years
        {
            get
            {
                var startYear = this.Periods.OrderBy(p => p.StartYear).First().StartYear;
                var endYear = this.Periods.OrderByDescending(p => p.EndYear).First().EndYear;

                return startYear == endYear ? startYear : $"{startYear}-{endYear}";
            }
        }

        public Func<string, OperationResult<bool>> VerifyChannel
            => this.Verify(nameof(this.Channel));

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
                (() => this.Periods.Select(p => p.ToString()).OrderBy(p => p),
                 () => this.Season.Periods.Select(p => new PeriodFormItem(p).ToString()).OrderBy(p => p))
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

            foreach (var title in this.RemovedTitles)
            {
                this.Season.Titles.Remove(title.Title);
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
            => sidePanel.OpenSeriesComponent.ExecuteIfCan(this);

        public override void WriteOrdinalNumber()
            => this.Season.OrdinalNumber = this.OrdinalNumber;

        public override string ToString()
            => $"{this.OrdinalNumber}; " +
                $"{String.Join("; ", this.Titles.Select(t => t.Name))}; " +
                $"{String.Join("; ", this.OriginalTitles.Select(t => t.Name))}" +
                $"{this.Channel}; {String.Join("; ", this.Periods.Select(p => p.ToString()))}; " +
                $"{this.IsWatched}; {this.IsReleased}";

        private void CopySeasonProperties()
        {
            this.CopyTitles(this.Season.Titles);

            this.IsWatched = this.Season.IsWatched;
            this.isReleased = this.Season.IsReleased;
            this.Channel = this.Season.Channel;
            this.OrdinalNumber = this.Season.OrdinalNumber;
            this.Periods = new ObservableCollection<PeriodFormItem>(
                this.Season.Periods.Select(period => this.NewPeriod(period)));

            this.SetPosters();
        }

        private void SetPosters()
        {
            this.Posters = new ObservableCollection<BitmapImage>(this.Periods
                .Select(p => p.PosterUrl)
                .Where(url => url != null)
                .Select(url =>
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(url, UriKind.Absolute);
                    bitmap.EndInit();

                    return bitmap;
                }));
        }

        private PeriodFormItem NewPeriod(Period period)
        {
            var result = new PeriodFormItem(period);
            result.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Periods));
            return result;
        }

        private void OnRemovePeriod(object obj)
        {
            if (obj is PeriodFormItem period)
            {
                this.Periods.Remove(period);
            }
        }

        private bool CanRemovePeriod()
            => this.Periods.Count != 1;
    }
}
