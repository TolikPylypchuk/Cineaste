using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeriesFormViewModel : TitledFormViewModelBase<Series, SeriesFormViewModel>
    {
        public SeriesFormViewModel(
            Series series,
            ReadOnlyObservableCollection<Kind> kinds,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Series = series;
            this.Kinds = kinds;

            this.CopyProperties();

            this.ImdbLinkRule = this.ValidationRule(vm => vm.ImdbLink, link => link.IsUrl(), "ImdbLinkInvalid");
            this.PosterUrlRule = this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

            this.CanDeleteWhenNotNew();

            this.Close = ReactiveCommand.Create(() => { });

            this.EnableChangeTracking();
        }

        public Series Series { get; }

        public ReadOnlyObservableCollection<Kind> Kinds { get; }

        [Reactive]
        public Kind Kind { get; set; } = null!;

        [Reactive]
        public bool IsWatched { get; set; }

        [Reactive]
        public bool IsAnthology { get; set; }

        [Reactive]
        public SeriesStatus Status { get; set; }

        [Reactive]
        public string ImdbLink { get; set; } = String.Empty;

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        public ValidationHelper ImdbLinkRule { get; }
        public ValidationHelper PosterUrlRule { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }

        public override bool IsNew
            => this.Series.Id == default;

        protected override SeriesFormViewModel Self
            => this;

        protected override IEnumerable<Title> ItemTitles
            => this.Series.Titles;

        protected override string NewItemKey
            => "NewSeries";

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Status, vm => vm.Series.Status);
            this.TrackChanges(vm => vm.Kind, vm => vm.Series.Kind);
            this.TrackChanges(vm => vm.IsWatched, vm => vm.Series.IsWatched);
            this.TrackChanges(vm => vm.IsAnthology, vm => vm.Series.IsAnthology);
            this.TrackChanges(vm => vm.ImdbLink, vm => vm.Series.ImdbLink.EmptyIfNull());
            this.TrackChanges(vm => vm.PosterUrl, vm => vm.Series.PosterUrl.EmptyIfNull());

            this.TrackValidation(this.ImdbLinkRule);
            this.TrackValidation(this.PosterUrlRule);

            base.EnableChangeTracking();
        }

        protected override Task<Series?> OnDeleteAsync()
            => Task.FromResult<Series?>(null);

        protected override Task<Series> OnSaveAsync()
            => Task.FromResult(this.Series);

        protected override void CopyProperties()
        {
            this.TitlesSource.Clear();
            this.TitlesSource.AddRange(this.Series.Titles);

            this.Status = this.Series.Status;
            this.Kind = this.Series.Kind;
            this.IsWatched = this.Series.IsWatched;
            this.IsAnthology = this.Series.IsAnthology;
            this.ImdbLink = this.Series.ImdbLink.EmptyIfNull();
            this.PosterUrl = this.Series.PosterUrl.EmptyIfNull();
        }

        protected override void AttachTitle(Title title)
            => title.Series = this.Series;
    }
}
