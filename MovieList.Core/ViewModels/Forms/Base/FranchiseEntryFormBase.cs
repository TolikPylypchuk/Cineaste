using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Forms.Base
{
#nullable disable
    public abstract class FranchiseEntryFormBase<TModel, TViewModel>
        : TitledFormBase<TModel, TViewModel>, IFranchiseEntryForm
#nullable enable
        where TModel : class
        where TViewModel : FranchiseEntryFormBase<TModel, TViewModel>
    {
        protected readonly SourceCache<Tag, int> TagsSource = new(tag => tag.Id);
        protected readonly SourceCache<Tag, int> AddableTagsSource = new(tag => tag.Id);

        private readonly ReadOnlyObservableCollection<TagItemViewModel> tags;
        private readonly ReadOnlyObservableCollection<AddableTagViewModel> addableTags;

        private readonly BehaviorSubject<bool> canCreateFranchiseSubject = new BehaviorSubject<bool>(false);

        protected FranchiseEntryFormBase(
            FranchiseEntry? entry,
            ReadOnlyObservableCollection<Tag> tags,
            ResourceManager? resourceManager,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.FranchiseEntry = entry;

            this.TagsSource.Connect()
                .Transform(this.CreateTagItem)
                .Sort(SortExpressionComparer<TagItemViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.tags)
                .DisposeMany()
                .Subscribe();

            this.AddableTagsSource.Connect()
                .Transform(tag => new AddableTagViewModel(tag))
                .Sort(SortExpressionComparer<AddableTagViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.addableTags)
                .DisposeMany()
                .Subscribe();

            this.AddableTagsSource.AddOrUpdate(tags.Where(this.IsTagApplicable));

            this.tags.ActOnEveryObject(
                onAdd: vm => this.AddableTagsSource.Remove(vm.Tag),
                onRemove: vm => this.AddableTagsSource.AddOrUpdate(vm.Tag));

            var formNotChanged = this.FormChanged.Invert();

            int lastSequenceNumber = this.FranchiseEntry?.ParentFranchise
                .Entries
                .Select(e => (int?)e.SequenceNumber)
                .Max()
                ?? 0;

            var canGoToFranchise = this.IfFranchisePresent(() => formNotChanged);

            this.GoToFranchise = ReactiveCommand.Create<Unit, Franchise>(
                _ => this.FranchiseEntry!.ParentFranchise, canGoToFranchise);

            var canGoToNext = this.IfFranchisePresent(() =>
                this.FranchiseEntry!.SequenceNumber >= lastSequenceNumber
                    ? Observable.Return(false)
                    : formNotChanged);

            this.GoToNext = ReactiveCommand.Create<Unit, FranchiseEntry>(
                _ => this.FranchiseEntry!.ParentFranchise.Entries
                    .OrderBy(e => e.SequenceNumber)
                    .First(e => e.SequenceNumber > this.FranchiseEntry!.SequenceNumber),
                canGoToNext);

            var canGoToPrevious = this.IfFranchisePresent(() =>
                this.FranchiseEntry!.SequenceNumber == 1
                    ? Observable.Return(false)
                    : formNotChanged);

            this.GoToPrevious = ReactiveCommand.Create<Unit, FranchiseEntry>(
                _ => this.FranchiseEntry!.ParentFranchise.Entries
                    .OrderBy(e => e.SequenceNumber)
                    .Last(e => e.SequenceNumber < this.FranchiseEntry!.SequenceNumber),
                canGoToPrevious);

            this.CreateFranchise = ReactiveCommand.Create(() => { }, this.canCreateFranchiseSubject);

            this.AddTag = ReactiveCommand.Create<Tag>(this.TagsSource.AddOrUpdate);
        }

        public FranchiseEntry? FranchiseEntry { get; }

        public ReadOnlyObservableCollection<AddableTagViewModel> AddableTags
            => this.addableTags;

        public ReadOnlyObservableCollection<TagItemViewModel> Tags
            => this.tags;

        public ReactiveCommand<Unit, Franchise> GoToFranchise { get; }
        public ReactiveCommand<Unit, FranchiseEntry> GoToNext { get; }
        public ReactiveCommand<Unit, FranchiseEntry> GoToPrevious { get; }
        public ReactiveCommand<Unit, Unit> CreateFranchise { get; }
        public ReactiveCommand<Tag, Unit> AddTag { get; }

        protected abstract IEnumerable<Tag> ItemTags { get; }

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(this.Tags.ToObservableChangeSet()
                .Transform(vm => vm.Tag.Id)
                .ToCollection()
                .Select(ids => !ids.ToHashSet().SetEquals(this.ItemTags.Select(tag => tag.Id).ToHashSet())));

            base.EnableChangeTracking();
        }

        protected override void CopyProperties()
        {
            base.CopyProperties();

            this.TagsSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(this.ItemTags);
            });
        }

        protected abstract bool IsTagApplicable(Tag tag);

        protected void CanCreateFranchise()
            => Observable.CombineLatest(
                   Observable.Return(!this.IsNew).Merge(this.Save.Select(_ => true)),
                   Observable.Return(this.FranchiseEntry == null),
                   this.FormChanged.Invert())
                .AllTrue()
                .Subscribe(this.canCreateFranchiseSubject);

        private IObservable<bool> IfFranchisePresent(Func<IObservable<bool>> observableProvider)
            => this.FranchiseEntry == null
                ? Observable.Return(false)
                : observableProvider();

        private TagItemViewModel CreateTagItem(Tag tag)
        {
            var vm = new TagItemViewModel(tag, canSelect: false);

            var subscriptions = new CompositeDisposable();

            vm.Delete.Subscribe(() =>
            {
                this.TagsSource.Remove(tag);
                subscriptions.Dispose();
            }).DisposeWith(subscriptions);

            return vm;
        }
    }
}
