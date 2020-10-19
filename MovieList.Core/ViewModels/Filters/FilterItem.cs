using System.Collections.ObjectModel;
using System.Reactive;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using Filter = System.Func<MovieList.Core.ListItems.ListItem, bool>;

namespace MovieList.Core.ViewModels.Filters
{
    public abstract class FilterItem : ReactiveObject
    {
        protected static readonly Filter NoFilter = item => true;

        private protected FilterItem(
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            IEnumConverter<SeriesWatchStatus>? seriesWatchStatusConverter = null,
            IEnumConverter<SeriesReleaseStatus>? seriesReleaseStatusConverter = null)
        {
            this.Kinds = kinds;
            this.Tags = tags;

            this.SeriesWatchStatusConverter = seriesWatchStatusConverter
                ?? Locator.Current.GetService<IEnumConverter<SeriesWatchStatus>>();

            this.SeriesReleaseStatusConverter = seriesReleaseStatusConverter
                ?? Locator.Current.GetService<IEnumConverter<SeriesReleaseStatus>>();
        }

        [Reactive]
        public bool IsNegated { get; set; }

        public ReadOnlyObservableCollection<Kind> Kinds { get; }
        public ReadOnlyObservableCollection<Tag> Tags { get; }

        public IEnumConverter<SeriesWatchStatus> SeriesWatchStatusConverter { get; }
        public IEnumConverter<SeriesReleaseStatus> SeriesReleaseStatusConverter { get; }

        public abstract ReactiveCommand<Unit, Unit> Delete { get; }

        public abstract Filter CreateFilter();
    }
}
