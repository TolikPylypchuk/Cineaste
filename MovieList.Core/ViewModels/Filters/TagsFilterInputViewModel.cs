using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.Models;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class TagsFilterInputViewModel : FilterInput, IActivatableViewModel
    {
        private readonly SourceCache<Tag, int> tagsSource = new(tag => tag.Id);
        private readonly SourceCache<Tag, int> addableTagsSource = new(tag => tag.Id);

        private readonly ReadOnlyObservableCollection<TagItemViewModel> tags;
        private readonly ReadOnlyObservableCollection<AddableTagViewModel> addableTags;

        public TagsFilterInputViewModel(ReadOnlyObservableCollection<Tag> tags)
        {
            this.tagsSource.Connect()
                .Transform(this.CreateTagItem)
                .Sort(SortExpressionComparer<TagItemViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.tags)
                .DisposeMany()
                .Subscribe();

            this.addableTagsSource.Connect()
                .Transform(tag => new AddableTagViewModel(tag))
                .Sort(SortExpressionComparer<AddableTagViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.addableTags)
                .DisposeMany()
                .Subscribe();

            this.addableTagsSource.AddOrUpdate(tags);

            this.tags.ActOnEveryObject(
                onAdd: vm => this.addableTagsSource.Remove(vm.Tag),
                onRemove: vm => this.addableTagsSource.AddOrUpdate(vm.Tag));

            this.tagsSource.Connect()
                .Discard()
                .Subscribe(this.inputChanged);

            this.AddTag = ReactiveCommand.Create<Tag>(this.tagsSource.AddOrUpdate);

            this.WhenActivated(disposables =>
            {
                tags.ToObservableChangeSet()
                    .Transform(tag => new TagModel(tag))
                    .ActOnEveryObject(
                        onAdd: tm => this.addableTagsSource.AddOrUpdate(tm.Tag),
                        onRemove: tm =>
                        {
                            this.tagsSource.Remove(tm.Tag);
                            this.addableTagsSource.Remove(tm.Tag);
                        })
                    .DisposeWith(disposables);
            });
        }

        public ReadOnlyObservableCollection<TagItemViewModel> Tags
            => this.tags;

        public ReadOnlyObservableCollection<AddableTagViewModel> AddableTags
            => this.addableTags;

        public ReactiveCommand<Tag, Unit> AddTag { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        private TagItemViewModel CreateTagItem(Tag tag)
        {
            var vm = new TagItemViewModel(tag, canSelect: false);

            var subscriptions = new CompositeDisposable();

            vm.Delete.Subscribe(() =>
            {
                this.tagsSource.Remove(tag);
                subscriptions.Dispose();
            }).DisposeWith(subscriptions);

            return vm;
        }
    }
}
