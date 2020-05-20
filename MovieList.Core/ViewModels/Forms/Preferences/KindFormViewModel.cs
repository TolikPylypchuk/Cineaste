using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using MovieList.Data.Models;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels.Forms.Preferences
{
    public sealed class KindFormViewModel : ReactiveForm<Kind, KindFormViewModel>
    {
        public KindFormViewModel(Kind kind, ResourceManager? resourceManager, IScheduler? scheduler)
            : base(resourceManager, scheduler)
        {
            this.Kind = kind;

            this.CopyProperties();
            this.CanDeleteWhen(Observable.Return(kind.Movies.Count == 0 && kind.Series.Count == 0));
            this.EnableChangeTracking();
        }

        public Kind Kind { get; }

        [Reactive]
        public string Name { get; set; } = String.Empty;

        [Reactive]
        public string ColorForWatchedMovie { get; set; } = String.Empty;

        [Reactive]
        public string ColorForWatchedSeries { get; set; } = String.Empty;

        [Reactive]
        public string ColorForNotWatchedMovie { get; set; } = String.Empty;

        [Reactive]
        public string ColorForNotWatchedSeries { get; set; } = String.Empty;

        [Reactive]
        public string ColorForNotReleasedMovie { get; set; } = String.Empty;

        [Reactive]
        public string ColorForNotReleasedSeries { get; set; } = String.Empty;

        public override bool IsNew
            => this.Kind.Id == default;

        protected override KindFormViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Name, vm => vm.Kind.Name);
            this.TrackChanges(vm => vm.ColorForWatchedMovie, vm => vm.Kind.ColorForWatchedMovie);
            this.TrackChanges(vm => vm.ColorForWatchedSeries, vm => vm.Kind.ColorForWatchedSeries);
            this.TrackChanges(vm => vm.ColorForNotWatchedMovie, vm => vm.Kind.ColorForNotWatchedMovie);
            this.TrackChanges(vm => vm.ColorForNotWatchedSeries, vm => vm.Kind.ColorForNotWatchedSeries);
            this.TrackChanges(vm => vm.ColorForNotReleasedMovie, vm => vm.Kind.ColorForNotReleasedMovie);
            this.TrackChanges(vm => vm.ColorForNotReleasedSeries, vm => vm.Kind.ColorForNotReleasedSeries);

            base.EnableChangeTracking();
        }

        protected override Task<Kind> OnSaveAsync()
        {
            this.Name = this.Kind.Name;
            this.Kind.ColorForWatchedMovie = this.ColorForWatchedMovie;
            this.Kind.ColorForWatchedSeries = this.ColorForWatchedSeries;
            this.Kind.ColorForNotWatchedMovie = this.ColorForNotWatchedMovie;
            this.Kind.ColorForNotWatchedSeries = this.ColorForNotWatchedSeries;
            this.Kind.ColorForNotReleasedMovie = this.ColorForNotReleasedMovie;
            this.Kind.ColorForNotReleasedSeries = this.ColorForNotReleasedSeries;

            return Task.FromResult(this.Kind);
        }

        protected override Task<Kind?> OnDeleteAsync()
            => Task.FromResult((Kind?)this.Kind);

        protected override void CopyProperties()
        {
            this.Name = this.Kind.Name;
            this.ColorForWatchedMovie = this.Kind.ColorForWatchedMovie;
            this.ColorForWatchedSeries = this.Kind.ColorForWatchedSeries;
            this.ColorForNotWatchedMovie = this.Kind.ColorForNotWatchedMovie;
            this.ColorForNotWatchedSeries = this.Kind.ColorForNotWatchedSeries;
            this.ColorForNotReleasedMovie = this.Kind.ColorForNotReleasedMovie;
            this.ColorForNotReleasedSeries = this.Kind.ColorForNotReleasedSeries;
        }
    }
}
