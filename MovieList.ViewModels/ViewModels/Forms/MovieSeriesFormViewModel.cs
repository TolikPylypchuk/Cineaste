using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.DialogModels;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public sealed class MovieSeriesFormViewModel : MovieSeriesEntryFormBase<MovieSeries, MovieSeriesFormViewModel>
    {
        private readonly IEntityService<MovieSeries> movieSeriesService;

        private readonly SourceList<MovieSeriesEntry> entriesSource = new SourceList<MovieSeriesEntry>();
        private readonly ReadOnlyObservableCollection<MovieSeriesEntryViewModel> entries;

        public MovieSeriesFormViewModel(
            MovieSeries movieSeries,
            string fileName,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IEntityService<MovieSeries>? movieSeriesService = null)
            : base(movieSeries.Entry, resourceManager, scheduler)
        {
            this.MovieSeries = movieSeries;

            this.movieSeriesService = movieSeriesService ??
                Locator.Current.GetService<IEntityService<MovieSeries>>(fileName);

            this.entriesSource.Connect()
                .Transform(this.CreateEntryViewModel)
                .AutoRefresh(vm => vm.SequenceNumber)
                .Sort(SortExpressionComparer<MovieSeriesEntryViewModel>.Ascending(vm => vm.SequenceNumber))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.entries)
                .Subscribe();

            var formTitleWhenHasTitles = this.FormTitle;
            var formTitleWhenDoesNotHaveTitles = Observable.Return(this.GetFormTitle(this.MovieSeries));

            this.FormTitle =
                this.WhenAnyValue(vm => vm.HasTitles)
                    .Select(hasTitles => hasTitles ? formTitleWhenHasTitles : formTitleWhenDoesNotHaveTitles)
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler);

            this.PosterUrlRule = this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

            var canSelectEntry = this.FormChanged.Invert();

            this.SelectEntry = ReactiveCommand.Create<MovieSeriesEntryViewModel, MovieSeriesEntry>(
                vm => vm.Entry, canSelectEntry);

            this.DetachEntry = ReactiveCommand.Create<MovieSeriesEntry, MovieSeriesEntry>(this.OnDetachEntry);

            this.WhenAnyValue(vm => vm.HasTitles)
                .BindTo(this, vm => vm.ShowTitles);

            this.WhenAnyValue(vm => vm.HasTitles)
                .Where(hasTitles => hasTitles && this.Titles.Count == 0)
                .Discard()
                .SubscribeAsync(this.AddTitles);

            this.WhenAnyValue(vm => vm.HasTitles)
                .Where(hasTitles => !hasTitles && this.Titles.Count > 0)
                .Discard()
                .Subscribe(this.ClearTitles);

            this.CopyProperties();
            this.CanDeleteWhenNotChanged();
            this.CanCreateMovieSeries();
            this.EnableChangeTracking();
        }

        public MovieSeries MovieSeries { get; }

        public ReadOnlyObservableCollection<MovieSeriesEntryViewModel> Entries
            => this.entries;

        [Reactive]
        public bool HasTitles { get; set; }

        [Reactive]
        public bool ShowTitles { get; set; }

        [Reactive]
        public bool IsLooselyConnected { get; set; }

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        public ValidationHelper PosterUrlRule { get; }

        public ReactiveCommand<MovieSeriesEntryViewModel, MovieSeriesEntry> SelectEntry { get; }
        public ReactiveCommand<MovieSeriesEntry, MovieSeriesEntry> DetachEntry { get; }

        public override bool IsNew
            => this.MovieSeries.Id == default;

        protected override MovieSeriesFormViewModel Self
            => this;

        protected override ICollection<Title> ItemTitles
            => this.MovieSeries.Titles;

        protected override string NewItemKey
            => "NewMovieSeries";

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.HasTitles, vm => vm.MovieSeries.Titles.Count > 0);
            this.TrackChanges(vm => vm.ShowTitles, vm => vm.MovieSeries.ShowTitles);
            this.TrackChanges(vm => vm.IsLooselyConnected, vm => vm.MovieSeries.IsLooselyConnected);
            this.TrackChanges(vm => vm.PosterUrl, vm => vm.MovieSeries.PosterUrl.EmptyIfNull());

            this.TrackChanges(this.IsCollectionChanged(vm => vm.Entries, vm => vm.MovieSeries.Entries));

            base.EnableChangeTracking();
        }

        protected override async Task<MovieSeries> OnSaveAsync()
        {
            await this.SaveTitlesAsync();

            foreach (var entry in this.Entries)
            {
                await entry.Save.Execute();
            }

            foreach (var entry in this.entriesSource.Items.Except(this.MovieSeries.Entries).ToList())
            {
                this.MovieSeries.Entries.Add(entry);
            }

            foreach (var entry in this.MovieSeries.Entries.Except(this.entriesSource.Items).ToList())
            {
                this.MovieSeries.Entries.Remove(entry);
            }

            this.MovieSeries.ShowTitles = this.ShowTitles;
            this.MovieSeries.IsLooselyConnected = this.IsLooselyConnected;
            this.MovieSeries.PosterUrl = this.PosterUrl.NullIfEmpty();

            await this.movieSeriesService.SaveAsync(this.MovieSeries);

            return this.MovieSeries;
        }

        protected override async Task<MovieSeries?> OnDeleteAsync()
        {
            bool shouldDelete = await Dialog.Confirm.Handle(new ConfirmationModel("DeleteMovieSeries"));

            if (shouldDelete)
            {
                await this.movieSeriesService.DeleteAsync(this.MovieSeries);
                return this.MovieSeries;
            }

            return null;
        }

        protected override void CopyProperties()
        {
            base.CopyProperties();

            this.entriesSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.MovieSeries.Entries);
            });

            this.HasTitles = this.MovieSeries.Titles.Count > 0;
            this.ShowTitles = this.MovieSeries.ShowTitles;
            this.IsLooselyConnected = this.MovieSeries.IsLooselyConnected;
            this.PosterUrl = this.MovieSeries.PosterUrl.EmptyIfNull();
        }

        protected override void AttachTitle(Title title)
            => title.MovieSeries = this.MovieSeries;

        private string GetFormTitle(MovieSeries movieSeries)
        {
            string title = movieSeries.ActualTitles.First(t => !t.IsOriginal).Name;
            return movieSeries.Entry == null ? title : $"{this.GetFormTitle(movieSeries.Entry.ParentSeries)}: {title}";
        }

        private async Task AddTitles()
        {
            string titleName = this.MovieSeries.ActualTitles.First(t => !t.IsOriginal).Name;
            string originalTitleName = this.MovieSeries.ActualTitles.First(t => t.IsOriginal).Name;

            await this.AddTitle.Execute();
            await this.AddOriginalTitle.Execute();

            this.Titles[0].Name = titleName;
            this.OriginalTitles[0].Name = originalTitleName;
        }

        private MovieSeriesEntryViewModel CreateEntryViewModel(MovieSeriesEntry entry)
        {
            var viewModel = new MovieSeriesEntryViewModel(entry, this, this.ResourceManager, this.Scheduler);
            var subsciptions = new CompositeDisposable();

            viewModel.Select
                .Select(_ => viewModel)
                .InvokeCommand(this.SelectEntry)
                .DisposeWith(subsciptions);

            viewModel.MoveUp
                .Select(_ => viewModel)
                .Subscribe(this.MoveEntryUp)
                .DisposeWith(subsciptions);

            viewModel.MoveDown
                .Select(_ => viewModel)
                .Subscribe(this.MoveEntryDown)
                .DisposeWith(subsciptions);

            viewModel.HideDisplayNumber
                .Select(_ => viewModel)
                .Subscribe(this.HideDisplayNumber)
                .DisposeWith(subsciptions);

            viewModel.ShowDisplayNumber
                .Select(_ => viewModel)
                .Subscribe(this.ShowDisplayNumber)
                .DisposeWith(subsciptions);

            viewModel.Delete
                .WhereNotNull()
                .InvokeCommand(this.DetachEntry)
                .DisposeWith(subsciptions);

            viewModel.Delete
                .WhereNotNull()
                .Discard()
                .Subscribe(subsciptions.Dispose)
                .DisposeWith(subsciptions);

            return viewModel;
        }

        private void MoveEntryUp(MovieSeriesEntryViewModel vm)
            => this.SwapEntryNumbers(this.Entries.First(e => e.SequenceNumber == vm.SequenceNumber - 1), vm);

        private void MoveEntryDown(MovieSeriesEntryViewModel vm)
            => this.SwapEntryNumbers(vm, this.Entries.First(e => e.SequenceNumber == vm.SequenceNumber + 1));

        private void SwapEntryNumbers(MovieSeriesEntryViewModel first, MovieSeriesEntryViewModel second)
        {
            first.SequenceNumber++;
            second.SequenceNumber--;

            if (first.DisplayNumber.HasValue)
            {
                first.DisplayNumber++;
            }

            if (second.DisplayNumber.HasValue)
            {
                second.DisplayNumber--;
            }
        }

        private MovieSeriesEntry OnDetachEntry(MovieSeriesEntry entry)
        {
            this.DecrementNumbers(entry.SequenceNumber);

            this.Entries
                .Where(e => e.SequenceNumber >= entry.SequenceNumber)
                .ForEach(e => e.SequenceNumber--);

            this.entriesSource.Remove(entry);

            return entry;
        }

        private void HideDisplayNumber(MovieSeriesEntryViewModel vm)
        {
            vm.DisplayNumber = null;
            this.DecrementNumbers(vm.SequenceNumber);
        }

        private void ShowDisplayNumber(MovieSeriesEntryViewModel vm)
        {
            vm.DisplayNumber = (this.Entries
                .Where(entry => entry.SequenceNumber < vm.SequenceNumber && entry.DisplayNumber.HasValue)
                .Select(entry => entry.DisplayNumber)
                .Max() ?? 0) + 1;

            this.IncrementNumbers(vm.SequenceNumber);
        }

        private void IncrementNumbers(int num)
            => this.UpdateNumbers(num, n => n + 1);

        private void DecrementNumbers(int num)
            => this.UpdateNumbers(num, n => n - 1);

        private void UpdateNumbers(int num, Func<int, int> update)
            => this.Entries
                .Where(entry => entry.SequenceNumber > num && entry.DisplayNumber.HasValue)
                .ForEach(entry => entry.DisplayNumber = update(entry.DisplayNumber ?? 0));
    }
}
