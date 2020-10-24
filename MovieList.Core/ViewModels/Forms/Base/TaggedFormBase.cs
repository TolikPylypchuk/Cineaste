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
using DynamicData.Binding;

using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Forms.Base
{
    public abstract class TaggedFormBase<TModel, TForm> : FranchiseEntryFormBase<TModel, TForm>
        where TModel : class
        where TForm : TaggedFormBase<TModel, TForm>
    {
        protected readonly SourceCache<Tag, int> TagsSource = new(tag => tag.Id);
        protected readonly SourceCache<Tag, int> AddableTagsSource = new(tag => tag.Id);

        private readonly ReadOnlyObservableCollection<TagItemViewModel> tags;
        private readonly ReadOnlyObservableCollection<AddableTagViewModel> addableTags;

        protected TaggedFormBase(
            FranchiseEntry? entry,
            ReadOnlyObservableCollection<Tag> tags,
            ResourceManager? resourceManager,
            IScheduler? scheduler = null)
            : base(entry, resourceManager, scheduler)
        {
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

            this.AddTag = ReactiveCommand.Create<Tag>(this.TagsSource.AddOrUpdate);
        }

        public ReadOnlyObservableCollection<AddableTagViewModel> AddableTags
            => this.addableTags;

        public ReadOnlyObservableCollection<TagItemViewModel> Tags
            => this.tags;

        protected abstract IEnumerable<Tag> ItemTags { get; }

        public ReactiveCommand<Tag, Unit> AddTag { get; }

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
