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
    public class SeriesFormItem : TitledFormItemBase
    {
        private bool isWatched;
        private bool isMiniseries;
        private SeriesStatus status;
        private string? imdbLink;
        private string? posterUrl;
        private KindViewModel kind;
        private BitmapImage? poster;

        private ObservableCollection<SeriesComponentFormItemBase> components;

        public SeriesFormItem(Series series, IEnumerable<KindViewModel> allKinds)
        {
            this.Series = series;
            this.AllKinds = allKinds;

            this.CopySeriesProperties();
            this.IsInitialized = true;
        }

        public Series Series { get; }
        public IEnumerable<KindViewModel> AllKinds { get; }

        public IEnumerable<SeriesStatus> AllStatuses { get; } =
            Enum.GetValues(typeof(SeriesStatus)).Cast<SeriesStatus>().ToList();

        public bool IsWatched
        {
            get => this.isWatched;
            set
            {
                this.isWatched = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsMiniseries
        {
            get => this.isMiniseries;
            set
            {
                this.isMiniseries = value;
                this.OnPropertyChanged();
            }
        }

        public SeriesStatus Status
        {
            get => this.status;
            set
            {
                this.status = value;
                this.OnPropertyChanged();

                if (this.status == SeriesStatus.NotStarted)
                {
                    this.IsWatched = false;
                }
            }
        }

        [Url(
            ErrorMessageResourceName = nameof(Messages.InvalidImdbLink),
            ErrorMessageResourceType = typeof(Messages))]
        public string? ImdbLink
        {
            get => this.imdbLink;
            set
            {
                this.imdbLink = value;
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

        public ObservableCollection<SeriesComponentFormItemBase> Components
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

        public Func<string, OperationResult<bool>> VerifyImdbLink
            => this.Verify(nameof(this.ImdbLink));

        public Func<string, OperationResult<bool>> VerifyPosterUrl
            => this.Verify(nameof(this.PosterUrl));

        protected override IEnumerable<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Titles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.Series.Titles.Where(t => !t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.OriginalTitles.OrderBy(t => t.Priority).Select(t => t.Name),
                 () => this.Series.Titles.Where(t => t.IsOriginal).OrderBy(t => t.Priority).Select(t => t.Name)),
                (() => this.IsWatched, () => this.Series.IsWatched),
                (() => this.IsMiniseries, () => this.Series.IsMiniseries),
                (() => this.Status, () => this.Series.Status),
                (() => this.ImdbLink, () => this.Series.ImdbLink),
                (() => this.PosterUrl, () => this.Series.PosterUrl),
                (() => this.Kind.Kind.Id, () => this.Series.KindId),
                (() => this.Components.Select(c => c.ToString()).OrderBy(c => c),
                 () => this.Series.Seasons
                    .Select(s => new SeasonFormItem(s).ToString())
                    .Union(this.Series.SpecialEpisodes.Select(e => new SpecialEpisodeFormItem(e).ToString()))
                    .OrderBy(c => c))
            };

        public override void WriteChanges()
        {
            if (this.Series.Id == default)
            {
                this.Series.Titles.Clear();
            }

            foreach (var title in this.Titles.Union(this.OriginalTitles))
            {
                title.WriteChanges();

                if (title.Title.Id == default)
                {
                    this.Series.Titles.Add(title.Title);
                }
            }
            
            foreach (var title in this.RemovedTitles)
            {
                this.Series.Titles.Remove(title.Title);
            }

            this.RemovedTitles.Clear();

            foreach (var component in this.Components)
            {
                component.RemovedTitles.Clear();
                component.WriteOrdinalNumber();
            }

            foreach (var season in this.Components.OfType<SeasonFormItem>())
            {
                if (season.Season.Id == default)
                {
                    this.Series.Seasons.Add(season.Season);
                }
            }

            foreach (var episode in this.Components.OfType<SpecialEpisodeFormItem>())
            {
                if (episode.SpecialEpisode.Id == default)
                {
                    this.Series.SpecialEpisodes.Add(episode.SpecialEpisode);
                }
            }

            if (this.Series.PosterUrl != this.PosterUrl)
            {
                this.SetPoster();
            }

            this.Series.IsWatched = this.IsWatched;
            this.Series.IsMiniseries = this.IsMiniseries;
            this.Series.Status = this.Status;
            this.Series.ImdbLink = this.ImdbLink;
            this.Series.PosterUrl = this.PosterUrl;
            this.Series.Kind = this.Kind.Kind;
            this.Series.KindId = this.Kind.Kind.Id;

            this.AreChangesPresent = false;
        }

        public override void RevertChanges()
        {
            this.CopySeriesProperties();
            this.AreChangesPresent = false;
        }

        private void CopySeriesProperties()
        {
            this.CopyTitles(this.Series.Titles);

            this.IsWatched = this.Series.IsWatched;
            this.IsMiniseries = this.Series.IsMiniseries;
            this.Status = this.Series.Status;
            this.ImdbLink = this.Series.ImdbLink;
            this.PosterUrl = this.Series.PosterUrl;
            this.Kind = this.AllKinds.FirstOrDefault(k => k.Kind.Id == this.Series.KindId) ?? this.AllKinds.First();
            this.Components = new ObservableCollection<SeriesComponentFormItemBase>(
                this.Series.Seasons
                    .Select(season => this.NewSeason(season))
                    .Cast<SeriesComponentFormItemBase>()
                    .Union(this.Series.SpecialEpisodes.Select(episode => this.NewSpecialEpisode(episode)))
                    .OrderBy(item => item.OrdinalNumber));

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

        private SeasonFormItem NewSeason(Season season)
        {
            var result = new SeasonFormItem(season);
            result.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Components));
            return result;
        }

        private SpecialEpisodeFormItem NewSpecialEpisode(SpecialEpisode episode)
        {
            var result = new SpecialEpisodeFormItem(episode);
            result.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Components));
            return result;
        }
    }
}
