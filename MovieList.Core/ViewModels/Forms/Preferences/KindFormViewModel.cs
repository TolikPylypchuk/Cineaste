using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using MovieList.Core.Validation;
using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data.Models;

using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public sealed class KindFormViewModel : ReactiveForm<Kind, KindFormViewModel>
    {
        private readonly BehaviorSubject<bool> isNew = new BehaviorSubject<bool>(true);

        public KindFormViewModel(
            Kind kind,
            IObservable<bool> isNew,
            IObservable<bool> canDelete,
            ResourceManager? resourceManager,
            IScheduler? scheduler)
            : base(resourceManager, scheduler)
        {
            this.Kind = kind;
            this.CopyProperties();

            this.NameRule = this.ValidationRule(vm => vm.Name, name => !String.IsNullOrWhiteSpace(name), "NameEmpty");
            this.ColorForWatchedMovieRule = this.ValidationRuleForColor(vm => vm.ColorForWatchedMovie);
            this.ColorForNotWatchedMovieRule = this.ValidationRuleForColor(vm => vm.ColorForNotWatchedMovie);
            this.ColorForNotReleasedMovieRule = this.ValidationRuleForColor(vm => vm.ColorForNotReleasedMovie);
            this.ColorForWatchedSeriesRule = this.ValidationRuleForColor(vm => vm.ColorForWatchedSeries);
            this.ColorForNotWatchedSeriesRule = this.ValidationRuleForColor(vm => vm.ColorForNotWatchedSeries);
            this.ColorForNotReleasedSeriesRule = this.ValidationRuleForColor(vm => vm.ColorForNotReleasedSeries);

            isNew.Subscribe(this.isNew);

            bool hasItems = kind.Movies.Count != 0 || kind.Series.Count != 0;

            this.CanDeleteWhen(hasItems ? Observable.Return(false) : canDelete);
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

        public ValidationHelper NameRule { get; }
        public ValidationHelper ColorForWatchedMovieRule { get; }
        public ValidationHelper ColorForNotWatchedMovieRule { get; }
        public ValidationHelper ColorForNotReleasedMovieRule { get; }
        public ValidationHelper ColorForWatchedSeriesRule { get; }
        public ValidationHelper ColorForNotWatchedSeriesRule { get; }
        public ValidationHelper ColorForNotReleasedSeriesRule { get; }

        public override bool IsNew
            => this.isNew.Value;

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

        protected override IObservable<Kind> OnSave()
        {
            this.Kind.Name = this.Name;
            this.Kind.ColorForWatchedMovie = this.ColorForWatchedMovie;
            this.Kind.ColorForWatchedSeries = this.ColorForWatchedSeries;
            this.Kind.ColorForNotWatchedMovie = this.ColorForNotWatchedMovie;
            this.Kind.ColorForNotWatchedSeries = this.ColorForNotWatchedSeries;
            this.Kind.ColorForNotReleasedMovie = this.ColorForNotReleasedMovie;
            this.Kind.ColorForNotReleasedSeries = this.ColorForNotReleasedSeries;

            return Observable.Return(this.Kind);
        }

        protected override IObservable<Kind?> OnDelete()
            => Observable.Return(this.Kind);

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

        private ValidationHelper ValidationRuleForColor(Expression<Func<KindFormViewModel, string>> vmProperty)
            => this.ValidationRule(vmProperty, HexColorValidator.IsArgbString, "HexColorInvalid");
    }
}
