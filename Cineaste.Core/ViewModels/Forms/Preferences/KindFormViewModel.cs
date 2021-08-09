using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using Cineaste.Core.Theming;
using Cineaste.Core.ViewModels.Forms.Base;
using Cineaste.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Core.ViewModels.Forms.Preferences
{
    public sealed class KindFormViewModel : ReactiveForm<Kind, KindFormViewModel>, IDisposable
    {
        private readonly IThemeAwareColorGenerator themeAwareColorGenerator;
        private readonly BehaviorSubject<bool> isNew = new(true);
        private readonly CompositeDisposable colorSubscriptions = new();

        public KindFormViewModel(
            Kind kind,
            IObservable<bool> isNew,
            IObservable<bool> canDelete,
            IObservable<IReadOnlyCollection<KindFormViewModel>> allKinds,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IThemeAwareColorGenerator? themeAwareColorGenerator = null)
            : base(resourceManager, scheduler)
        {
            this.themeAwareColorGenerator = themeAwareColorGenerator ?? GetDefaultService<IThemeAwareColorGenerator>();

            this.Kind = kind;
            this.CopyProperties();

            var nameAndKindsObservable = Observable.CombineLatest(
                allKinds,
                this.WhenAnyValue(vm => vm.Name),
                (kinds, name) => (Kinds: kinds, Name: name));

            this.ValidationRule(
                nameAndKindsObservable,
                nameAndKinds =>
                    !String.IsNullOrWhiteSpace(nameAndKinds.Name) &&
                    nameAndKinds.Kinds.Count(k => k.Name == nameAndKinds.Name) == 1,
                nameAndKinds => String.IsNullOrWhiteSpace(nameAndKinds.Name) ? "NameEmpty" : "NameNotUnique");

            this.ValidationRuleForColor(vm => vm.ColorForWatchedMovie);
            this.ValidationRuleForColor(vm => vm.ColorForNotWatchedMovie);
            this.ValidationRuleForColor(vm => vm.ColorForNotReleasedMovie);
            this.ValidationRuleForColor(vm => vm.ColorForWatchedSeries);
            this.ValidationRuleForColor(vm => vm.ColorForNotWatchedSeries);
            this.ValidationRuleForColor(vm => vm.ColorForNotReleasedSeries);

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

        public override bool IsNew => this.isNew.Value;

        protected override KindFormViewModel Self => this;

        public void Dispose() =>
            this.colorSubscriptions.Dispose();

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Name, vm => vm.Kind.Name);

            this.TrackChanges(
                vm => vm.ColorForWatchedMovie,
                vm => this.themeAwareColorGenerator.TransformColorForCurrentTheme(vm.Kind.ColorForWatchedMovie));

            this.TrackChanges(
                vm => vm.ColorForWatchedSeries,
                vm => this.themeAwareColorGenerator.TransformColorForCurrentTheme(vm.Kind.ColorForWatchedSeries));

            this.TrackChanges(
                vm => vm.ColorForNotWatchedMovie,
                vm => this.themeAwareColorGenerator.TransformColorForCurrentTheme(vm.Kind.ColorForNotWatchedMovie));

            this.TrackChanges(
                vm => vm.ColorForNotWatchedSeries,
                vm => this.themeAwareColorGenerator.TransformColorForCurrentTheme(vm.Kind.ColorForNotWatchedSeries));

            this.TrackChanges(
                vm => vm.ColorForNotReleasedMovie,
                vm => this.themeAwareColorGenerator.TransformColorForCurrentTheme(vm.Kind.ColorForNotReleasedMovie));

            this.TrackChanges(
                vm => vm.ColorForNotReleasedSeries,
                vm => this.themeAwareColorGenerator.TransformColorForCurrentTheme(vm.Kind.ColorForNotReleasedSeries));

            base.EnableChangeTracking();
        }

        protected override IObservable<Kind> OnSave()
        {
            this.Kind.Name = this.Name;

            this.Kind.ColorForWatchedMovie = this.themeAwareColorGenerator.TransformColorForCurrentTheme(
                this.ColorForWatchedMovie);

            this.Kind.ColorForWatchedSeries = this.themeAwareColorGenerator.TransformColorForCurrentTheme(
                this.ColorForWatchedSeries);

            this.Kind.ColorForNotWatchedMovie = this.themeAwareColorGenerator.TransformColorForCurrentTheme(
                this.ColorForNotWatchedMovie);

            this.Kind.ColorForNotWatchedSeries = this.themeAwareColorGenerator.TransformColorForCurrentTheme(
                this.ColorForNotWatchedSeries);

            this.Kind.ColorForNotReleasedMovie = this.themeAwareColorGenerator.TransformColorForCurrentTheme(
                this.ColorForNotReleasedMovie);

            this.Kind.ColorForNotReleasedSeries = this.themeAwareColorGenerator.TransformColorForCurrentTheme(
                this.ColorForNotReleasedSeries);

            return Observable.Return(this.Kind);
        }

        protected override IObservable<Kind?> OnDelete() =>
            Observable.Return(this.Kind);

        protected override void CopyProperties()
        {
            this.Name = this.Kind.Name;

            this.ColorForWatchedMovie = this.Kind.ColorForWatchedMovie;
            this.ColorForWatchedSeries = this.Kind.ColorForWatchedSeries;
            this.ColorForNotWatchedMovie = this.Kind.ColorForNotWatchedMovie;
            this.ColorForNotWatchedSeries = this.Kind.ColorForNotWatchedSeries;
            this.ColorForNotReleasedMovie = this.Kind.ColorForNotReleasedMovie;
            this.ColorForNotReleasedSeries = this.Kind.ColorForNotReleasedSeries;

            this.colorSubscriptions.Clear();

            this.MakeColorsThemeAware();
        }

        private void MakeColorsThemeAware()
        {
            this.themeAwareColorGenerator.CreateThemeAwareColor(this.ColorForWatchedMovie)
                .BindTo(this, vm => vm.ColorForWatchedMovie)
                .DisposeWith(this.colorSubscriptions);

            this.themeAwareColorGenerator.CreateThemeAwareColor(this.ColorForWatchedSeries)
                .BindTo(this, vm => vm.ColorForWatchedSeries)
                .DisposeWith(this.colorSubscriptions);

            this.themeAwareColorGenerator.CreateThemeAwareColor(this.ColorForNotWatchedMovie)
                .BindTo(this, vm => vm.ColorForNotWatchedMovie)
                .DisposeWith(this.colorSubscriptions);

            this.themeAwareColorGenerator.CreateThemeAwareColor(this.ColorForNotWatchedSeries)
                .BindTo(this, vm => vm.ColorForNotWatchedSeries)
                .DisposeWith(this.colorSubscriptions);

            this.themeAwareColorGenerator.CreateThemeAwareColor(this.ColorForNotReleasedMovie)
                .BindTo(this, vm => vm.ColorForNotReleasedMovie)
                .DisposeWith(this.colorSubscriptions);

            this.themeAwareColorGenerator.CreateThemeAwareColor(this.ColorForNotReleasedSeries)
                .BindTo(this, vm => vm.ColorForNotReleasedSeries)
                .DisposeWith(this.colorSubscriptions);
        }
    }
}
