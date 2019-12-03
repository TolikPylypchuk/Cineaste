using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.DialogModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeriesFormViewModel : TitledFormViewModelBase<Series, SeriesFormViewModel>
    {
        private readonly IEntityService<Series> seriesService;

        public SeriesFormViewModel(
            Series series,
            ReadOnlyObservableCollection<Kind> kinds,
            string fileName,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IEntityService<Series>? seriesService = null)
            : base(resourceManager, scheduler)
        {
            this.Series = series;
            this.Kinds = kinds;

            this.seriesService = seriesService ?? Locator.Current.GetService<IEntityService<Series>>(fileName);

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

        protected override async Task<Series> OnSaveAsync()
        {
            foreach (var title in this.Titles.Union(this.OriginalTitles))
            {
                await title.Save.Execute();
            }

            this.Series.Titles.Add(this.TitlesSource.Items.Except(this.Series.Titles).ToList());
            this.Series.Titles.Remove(this.Series.Titles.Except(this.TitlesSource.Items).ToList());

            this.Series.IsWatched = this.IsWatched;
            this.Series.IsAnthology = this.IsAnthology;
            this.Series.Status = this.Status;
            this.Series.Kind = this.Kind;
            this.Series.ImdbLink = this.ImdbLink.NullIfEmpty();
            this.Series.PosterUrl = this.PosterUrl.NullIfEmpty();

            await this.seriesService.SaveAsync(this.Series);

            return this.Series;
        }

        protected override async Task<Series?> OnDeleteAsync()
        {
            bool shouldDelete = await Dialog.Confirm.Handle(new ConfirmationModel("DeleteSeries"));

            if (shouldDelete)
            {
                await this.seriesService.DeleteAsync(this.Series);
                return this.Series;
            }

            return null;
        }

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
