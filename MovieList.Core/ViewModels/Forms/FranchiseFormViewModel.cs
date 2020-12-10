using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Resources;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using MovieList.Core.Comparers;
using MovieList.Core.Data.Services;
using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;

using Nito.Comparers;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.Core.ViewModels.Forms
{
    public sealed class FranchiseFormViewModel : FranchiseEntryFormBase<Franchise, FranchiseFormViewModel>
    {
        private readonly IEntityService<Franchise> franchiseService;

        private readonly SourceList<FranchiseEntry> entriesSource = new();
        private readonly ReadOnlyObservableCollection<FranchiseEntryViewModel> entries;

        private readonly SourceList<FranchiseEntry> addableItemsSource = new();
        private readonly ReadOnlyObservableCollection<FranchiseAddableItemViewModel> addableItems;

        public FranchiseFormViewModel(
            Franchise franchise,
            string fileName,
            IEnumerable<FranchiseEntry> addableItems,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IEntityService<Franchise>? franchiseService = null,
            Settings? settings = null)
            : base(franchise.Entry, resourceManager, scheduler)
        {
            this.Franchise = franchise;
            settings ??= Locator.Current.GetService<Settings>(fileName);

            this.franchiseService = franchiseService ??
                Locator.Current.GetService<IEntityService<Franchise>>(fileName);
            
            this.entriesSource.Connect()
                .Transform(this.CreateEntryViewModel)
                .AutoRefresh(vm => vm.SequenceNumber)
                .Sort(SortExpressionComparer<FranchiseEntryViewModel>.Ascending(vm => vm.SequenceNumber))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.entries)
                .Subscribe();

            this.addableItemsSource.Connect()
                .Filter(entry => entry.Franchise != this.Franchise)
                .Transform(item => new FranchiseAddableItemViewModel(item))
                .Sort(ComparerBuilder.For<FranchiseAddableItemViewModel>()
                    .OrderBy(vm => vm.Entry, new FranchiseEntryTitleComparer(settings.CultureInfo)))
                .Bind(out this.addableItems)
                .Subscribe();

            this.addableItemsSource.AddRange(addableItems);

            this.FormTitle = this.FormTitle
                .Select(this.GetFullFormTitle)
                .StartWith(this.GetFullFormTitle(
                    this.GetFormTitle(this.Franchise.ActualTitles.FirstOrDefault()?.Name ?? String.Empty)));

            this.PosterUrlRule = this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

            this.entriesSource.Connect()
                .Count()
                .Select(count => count > 0)
                .ToPropertyEx(this, vm => vm.CanShowTitles);

            var canSelectEntry = this.FormChanged.Invert();

            this.SelectEntry = ReactiveCommand.Create<FranchiseEntryViewModel, FranchiseEntry>(
                vm => vm.Entry, canSelectEntry);

            this.DetachEntry = ReactiveCommand.Create<FranchiseEntry, FranchiseEntry>(this.OnDetachEntry);

            var canAddEntry = Observable.CombineLatest(
                    Observable.Return(!this.IsNew).Merge(this.Save.Select(_ => true)),
                    this.FormChanged.Invert())
                .AllTrue();

            this.AddMovie = ReactiveCommand.Create(this.GetFirstDisplayNumber, canAddEntry);
            this.AddSeries = ReactiveCommand.Create(this.GetFirstDisplayNumber, canAddEntry);
            this.AddFranchise = ReactiveCommand.Create(this.GetFirstDisplayNumber, canAddEntry);
            this.AddExistingItem = ReactiveCommand.Create<FranchiseEntry, FranchiseEntry>(this.OnAddExistingItem);

            this.InitializeValueDependencies();
            this.CopyProperties();
            this.CanDeleteWhenNotChanged();
            this.CanCreateFranchise();
            this.EnableChangeTracking();
        }

        public Franchise Franchise { get; }

        public ReadOnlyObservableCollection<FranchiseEntryViewModel> Entries =>
            this.entries;

        public ReadOnlyObservableCollection<FranchiseAddableItemViewModel> AddableItems =>
            this.addableItems;

        [Reactive]
        public bool HasTitles { get; set; }

        [Reactive]
        public bool ShowTitles { get; set; }

        [Reactive]
        public bool IsLooselyConnected { get; set; }

        [Reactive]
        public bool MergeDisplayNumbers { get; set; }

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        public ValidationHelper PosterUrlRule { get; }

        public bool CanShowTitles { [ObservableAsProperty] get; }

        public ReactiveCommand<FranchiseEntryViewModel, FranchiseEntry> SelectEntry { get; }
        public ReactiveCommand<FranchiseEntry, FranchiseEntry> DetachEntry { get; }
        public ReactiveCommand<Unit, int> AddMovie { get; }
        public ReactiveCommand<Unit, int> AddSeries { get; }
        public ReactiveCommand<Unit, int> AddFranchise { get; }
        public ReactiveCommand<FranchiseEntry, FranchiseEntry> AddExistingItem { get; }

        public override bool IsNew =>
            this.Franchise.Id == default;

        protected override FranchiseFormViewModel Self => this;

        protected override ICollection<Title> ItemTitles =>
            this.Franchise.Titles;

        protected override string NewItemKey => "NewFranchise";

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.HasTitles, vm => vm.Franchise.Titles.Count > 0);
            this.TrackChanges(vm => vm.ShowTitles, vm => vm.Franchise.ShowTitles);
            this.TrackChanges(vm => vm.IsLooselyConnected, vm => vm.Franchise.IsLooselyConnected);
            this.TrackChanges(vm => vm.MergeDisplayNumbers, vm => vm.Franchise.MergeDisplayNumbers);
            this.TrackChanges(vm => vm.PosterUrl, vm => vm.Franchise.PosterUrl.EmptyIfNull());

            this.TrackChanges(this.IsCollectionChanged(vm => vm.Entries, vm => vm.Franchise.Entries));

            base.EnableChangeTracking();
        }

        protected override IObservable<Franchise> OnSave()
            => this.SaveTitles()
                .DoAsync(this.SaveEntries)
                .Select(this.CopyPropertiesIntoModel)
                .DoAsync(this.franchiseService.SaveInTaskPool);

        protected override IObservable<Franchise?> OnDelete() =>
            Dialog.PromptToDelete(
                "DeleteFranchise",
                () => this.franchiseService.DeleteInTaskPool(this.Franchise).Select(() => this.Franchise));

        protected override void CopyProperties()
        {
            base.CopyProperties();

            this.entriesSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.Franchise.Entries);
            });

            this.HasTitles = this.Franchise.Titles.Count > 0;
            this.ShowTitles = this.Franchise.ShowTitles;
            this.IsLooselyConnected = this.Franchise.IsLooselyConnected;
            this.MergeDisplayNumbers = this.Franchise.MergeDisplayNumbers;
            this.PosterUrl = this.Franchise.PosterUrl.EmptyIfNull();
        }

        protected override void AttachTitle(Title title) =>
            title.Franchise = this.Franchise;

        private void InitializeValueDependencies()
        {
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

            this.WhenAnyValue(vm => vm.ShowTitles)
                .Where(showTitles => showTitles && !this.CanShowTitles)
                .Subscribe(_ => this.ShowTitles = false);

            this.WhenAnyValue(vm => vm.MergeDisplayNumbers)
                .Subscribe(_ => this.AdjustDisplayNumbers(this.GetFirstDisplayNumber()));
        }

        private string GetFormTitle(Franchise franchise)
        {
            string title = franchise.ActualTitles.FirstOrDefault(t => !t.IsOriginal)?.Name ?? String.Empty;
            return franchise.Entry == null ? title : $"{this.GetFormTitle(franchise.Entry.ParentFranchise)}: {title}";
        }

        private string GetFullFormTitle(string title) =>
            this.FranchiseEntry != null
                ? $"{this.GetFormTitle(this.FranchiseEntry.ParentFranchise)}: {title}"
                : title;

        private IObservable<Unit> AddTitles()
        {
            string titleName = this.Franchise.ActualTitles.FirstOrDefault(t => !t.IsOriginal)?.Name
                ?? String.Empty;

            string originalTitleName = this.Franchise.ActualTitles.FirstOrDefault(t => t.IsOriginal)?.Name
                ?? String.Empty;

            return this.AddTitle.Execute()
                .DoAsync(() => this.AddOriginalTitle.Execute())
                .Do(() =>
                {
                    this.Titles[0].Name = titleName;
                    this.OriginalTitles[0].Name = originalTitleName;
                });
        }

        private FranchiseEntryViewModel CreateEntryViewModel(FranchiseEntry entry)
        {
            var viewModel = new FranchiseEntryViewModel(entry, this, this.ResourceManager, this.Scheduler);
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

        private void MoveEntryUp(FranchiseEntryViewModel vm) =>
            this.SwapEntryNumbers(this.Entries.First(e => e.SequenceNumber == vm.SequenceNumber - 1), vm);

        private void MoveEntryDown(FranchiseEntryViewModel vm) =>
            this.SwapEntryNumbers(vm, this.Entries.First(e => e.SequenceNumber == vm.SequenceNumber + 1));

        private void SwapEntryNumbers(FranchiseEntryViewModel first, FranchiseEntryViewModel second)
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

        private FranchiseEntry OnDetachEntry(FranchiseEntry entry)
        {
            this.DecrementNumbers(entry.SequenceNumber);

            this.Entries
                .Where(e => e.SequenceNumber >= entry.SequenceNumber)
                .ForEach(e => e.SequenceNumber--);

            this.entriesSource.Remove(entry);
            this.addableItemsSource.Add(entry);

            return entry;
        }

        private FranchiseEntry OnAddExistingItem(FranchiseEntry entry)
        {
            var newEntry = new FranchiseEntry
            {
                Movie = entry.Movie,
                Series = entry.Series,
                Franchise = entry.Franchise,
                ParentFranchise = this.Franchise,
                SequenceNumber = this.Entries
                    .OrderByDescending(e => e.SequenceNumber)
                    .Select(e => e.SequenceNumber)
                    .FirstOrDefault() + 1,
                DisplayNumber = (this.Entries
                    .Where(e => e.DisplayNumber != null)
                    .OrderByDescending(e => e.DisplayNumber)
                    .Select(e => e.DisplayNumber)
                    .FirstOrDefault() ?? 0) + 1
            };

            this.entriesSource.Add(newEntry);
            this.addableItemsSource.Remove(entry);

            return newEntry;
        }

        private void HideDisplayNumber(FranchiseEntryViewModel vm)
        {
            vm.DisplayNumber = null;
            this.DecrementNumbers(vm.SequenceNumber);
        }

        private void ShowDisplayNumber(FranchiseEntryViewModel vm)
        {
            vm.DisplayNumber = (this.Entries
                .Where(entry => entry.SequenceNumber < vm.SequenceNumber && entry.DisplayNumber.HasValue)
                .Select(entry => entry.DisplayNumber)
                .Max() ?? (this.GetFirstDisplayNumber() - 1)) + 1;

            this.IncrementNumbers(vm.SequenceNumber);
        }

        private void IncrementNumbers(int num) =>
            this.UpdateNumbers(num, n => n + 1);

        private void DecrementNumbers(int num) =>
            this.UpdateNumbers(num, n => n - 1);

        private void UpdateNumbers(int num, Func<int, int> update) =>
            this.Entries
                .Where(entry => entry.SequenceNumber > num && entry.DisplayNumber.HasValue)
                .ForEach(entry => entry.DisplayNumber = update(entry.DisplayNumber ?? 0));

        private int GetFirstDisplayNumber() =>
            this.MergeDisplayNumbers
                ? this.GetNextDisplayNumber(this.FranchiseEntry?.ParentFranchise.Entries
                    .LastOrDefault(entry => entry.SequenceNumber < this.FranchiseEntry.SequenceNumber))
                : 1;

        private void AdjustDisplayNumbers(int firstNumber) =>
            this.Entries
                .Where(entry => entry.DisplayNumber != null)
                .ForEach((entry, index) => entry.DisplayNumber = index + firstNumber);

        private int GetNextDisplayNumber(FranchiseEntry? entry)
        {
            if (entry == null)
            {
                return 1;
            }

            if (entry.DisplayNumber != null && (entry.Movie != null || entry.Series != null))
            {
                return entry.DisplayNumber.Value + 1;
            }

            if (entry.Franchise != null)
            {
                return (entry.Franchise.Entries.Select(e => e.DisplayNumber).Max() ?? 0) + 1;
            }

            return this.GetNextDisplayNumber(entry.ParentFranchise.Entries
                .LastOrDefault(e => e.SequenceNumber < entry.SequenceNumber));
        }

        private IObservable<Unit> SaveEntries() =>
            this.Entries.Count == 0
                ? Observable.Return(Unit.Default)
                : this.Entries
                    .Select(entry => entry.Save.Execute())
                    .ForkJoin()
                    .Discard();

        private Franchise CopyPropertiesIntoModel()
        {
            foreach (var entry in this.entriesSource.Items.Except(this.Franchise.Entries).ToList())
            {
                this.Franchise.Entries.Add(entry);
            }

            foreach (var entry in this.Franchise.Entries.Except(this.entriesSource.Items).ToList())
            {
                this.Franchise.Entries.Remove(entry);
            }

            this.Franchise.ShowTitles = this.ShowTitles;
            this.Franchise.IsLooselyConnected = this.IsLooselyConnected;
            this.Franchise.MergeDisplayNumbers = this.MergeDisplayNumbers;
            this.Franchise.PosterUrl = this.PosterUrl.NullIfEmpty();

            return this.Franchise;
        }
    }
}
