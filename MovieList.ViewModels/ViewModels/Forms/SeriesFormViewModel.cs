using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

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

            this.ImdbLinkRule = this.CreateImdbLinkRule();
            this.PosterUrlRule = this.CreatePosterUrlRule();

            Observable.Return(this.Series.Id != default)
                .Merge(this.Save.Select(_ => true))
                .Subscribe(this.CanDeleteSubject);

            this.Close = ReactiveCommand.Create(() => { });

            this.InitializeChangeTracking();
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

        protected override IEnumerable<Title> ItemTitles
            => this.Series.Titles;

        protected override string NewItemKey
            => "NewSeries";

        protected override void InitializeChangeTracking()
        {
            var statusChanged = this.WhenAnyValue(vm => vm.Status)
                .Select(status => status != this.Series.Status)
                .Do(changed => this.Log().Debug(changed ? "Status is changed" : "Status is unchanged"));

            var kindChanged = this.WhenAnyValue(vm => vm.Kind)
                .Select(kind => kind != this.Series.Kind)
                .Do(changed => this.Log().Debug(changed ? "Kind is changed" : "Kind is unchanged"));

            var isWatchedChanged = this.WhenAnyValue(vm => vm.IsWatched)
                .Select(isWatched => isWatched != this.Series.IsWatched)
                .Do(changed => this.Log().Debug(changed ? "Is watched is changed" : "Is watched is unchanged"));

            var isAnthologyChanged = this.WhenAnyValue(vm => vm.IsAnthology)
                .Select(isAnthology => isAnthology != this.Series.IsAnthology)
                .Do(changed => this.Log().Debug(changed ? "Is anthology is changed" : "Is anthology is unchanged"));

            var imdbLinkChanged = this.WhenAnyValue(vm => vm.ImdbLink)
                .Select(link => link.NullIfEmpty() != this.Series.ImdbLink)
                .Do(changed => this.Log().Debug(changed ? "IMDb link is changed" : "IMDb link is unchanged"));

            var posterUrlChanged = this.WhenAnyValue(vm => vm.PosterUrl)
                .Select(url => url.NullIfEmpty() != this.Series.PosterUrl)
                .Do(changed => this.Log().Debug(changed ? "Poster URL is changed" : "Poster URL is unchanged"));

            var falseWhenSave = this.Save.Select(_ => false);
            var falseWhenCancel = this.Cancel.Select(_ => false);

            Observable.CombineLatest(
                    this.TitlesChanged,
                    this.OriginalTitlesChanged,
                    statusChanged,
                    kindChanged,
                    isWatchedChanged,
                    isAnthologyChanged,
                    imdbLinkChanged,
                    posterUrlChanged)
                .AnyTrue()
                .Merge(falseWhenSave)
                .Merge(falseWhenCancel)
                .Subscribe(this.FormChangedSubject);

            Observable.CombineLatest(
                    this.TitlesValid,
                    this.OriginalTitlesValid,
                    this.ImdbLinkRule.Valid(),
                    this.PosterUrlRule.Valid())
                .AllTrue()
                .Subscribe(this.ValidSubject);
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

        private ValidationHelper CreateImdbLinkRule()
            => this.ValidationRule(
                vm => vm.ImdbLink,
                link => link.IsUrl(),
                this.ResourceManager.GetString("ValidationImdbLinkInvalid"));

        private ValidationHelper CreatePosterUrlRule()
            => this.ValidationRule(
                vm => vm.PosterUrl,
                url => url.IsUrl(),
                this.ResourceManager.GetString("ValidationPosterUrlInvalid"));
    }
}
