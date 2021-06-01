using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Cineaste.Data.Models;

using ReactiveUI;

using static Cineaste.Core.ServiceUtil;

using Filter = System.Func<Cineaste.Core.ListItems.ListItem, bool>;

namespace Cineaste.Core.ViewModels.Filters
{
    public abstract class FilterItem : ReactiveObject
    {
        protected static readonly Filter NoFilter = item => true;

        protected readonly Subject<Unit> FilterChangedSubject = new();

        private protected FilterItem(
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            IEnumConverter<SeriesWatchStatus>? seriesWatchStatusConverter = null,
            IEnumConverter<SeriesReleaseStatus>? seriesReleaseStatusConverter = null)
        {
            this.Kinds = kinds;
            this.Tags = tags;

            this.SeriesWatchStatusConverter = seriesWatchStatusConverter
                ?? GetDefaultService<IEnumConverter<SeriesWatchStatus>>();

            this.SeriesReleaseStatusConverter = seriesReleaseStatusConverter
                ?? GetDefaultService<IEnumConverter<SeriesReleaseStatus>>();
        }

        public IObservable<Unit> FilterChanged =>
            this.FilterChangedSubject.AsObservable();

        public ReadOnlyObservableCollection<Kind> Kinds { get; }
        public ReadOnlyObservableCollection<Tag> Tags { get; }

        public IEnumConverter<SeriesWatchStatus> SeriesWatchStatusConverter { get; }
        public IEnumConverter<SeriesReleaseStatus> SeriesReleaseStatusConverter { get; }

        public abstract ReactiveCommand<Unit, Unit> Delete { get; }

        public abstract Filter CreateFilter();
    }
}
