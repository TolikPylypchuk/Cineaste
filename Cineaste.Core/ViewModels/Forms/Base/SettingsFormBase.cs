using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using Cineaste.Core.Models;
using Cineaste.Core.Preferences;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Data;
using Cineaste.Data.Models;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.ViewModels.Forms.Base
{
    public abstract class SettingsFormBase<TSettings, TForm> : ReactiveForm<TSettings, TForm>
        where TSettings : class, ISettings
        where TForm : SettingsFormBase<TSettings, TForm>
    {
        private readonly SourceList<Kind> kindsSource = new();
        private readonly ReadOnlyObservableCollection<KindFormViewModel> kinds;

        private readonly SourceList<TagModel> tagsSource = new();
        private readonly ReadOnlyObservableCollection<TagItemViewModel> tagItems;

        protected SettingsFormBase(
            TSettings settings,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Model = settings;

            this.kindsSource.Connect()
                .Transform(this.CreateKindForm)
                .Bind(out this.kinds)
                .DisposeMany()
                .Subscribe();

            this.tagsSource.Connect()
                .Transform(this.CreateTagItem)
                .AutoRefresh(vm => vm.Category)
                .AutoRefresh(vm => vm.Name)
                .Sort(SortExpressionComparer<TagItemViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.tagItems)
                .DisposeMany()
                .Subscribe();

            this.AddKind = ReactiveCommand.Create(() => this.kindsSource.Add(new Kind()));
            this.AddTag = ReactiveCommand.CreateFromObservable(this.OnAddTag);
            this.OpenTagForm = ReactiveCommand.CreateFromObservable<TagItemViewModel, Unit>(this.OnOpenTagForm);

            this.DefaultSeasonTitle = String.Empty;
            this.DefaultSeasonOriginalTitle = String.Empty;
            this.CultureInfo = null!;
        }

        public TSettings Model { get; }

        [Reactive]
        public string DefaultSeasonTitle { get; set; }

        [Reactive]
        public string DefaultSeasonOriginalTitle { get; set; }

        [Reactive]
        public CultureInfo CultureInfo { get; set; }

        [Reactive]
        public ListSortOrder DefaultFirstSortOrder { get; set; }

        [Reactive]
        public ListSortOrder DefaultSecondSortOrder { get; set; }

        [Reactive]
        public ListSortDirection DefaultFirstSortDirection { get; set; }

        [Reactive]
        public ListSortDirection DefaultSecondSortDirection { get; set; }

        public ReadOnlyObservableCollection<KindFormViewModel> Kinds =>
            this.kinds;

        public ReadOnlyObservableCollection<TagItemViewModel> TagItems =>
            this.tagItems;

        public ReactiveCommand<Unit, Unit> AddKind { get; }
        public ReactiveCommand<Unit, Unit> AddTag { get; }
        public ReactiveCommand<TagItemViewModel, Unit> OpenTagForm { get; }

        public override bool IsNew => false;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.DefaultSeasonTitle, vm => vm.Model.DefaultSeasonTitle);
            this.TrackChanges(
                vm => vm.DefaultSeasonOriginalTitle, vm => vm.Model.DefaultSeasonOriginalTitle);

            this.TrackChanges(vm => vm.CultureInfo, vm => vm.Model.CultureInfo);

            this.TrackChanges(vm => vm.DefaultFirstSortOrder, vm => vm.Model.DefaultFirstSortOrder);
            this.TrackChanges(vm => vm.DefaultSecondSortOrder, vm => vm.Model.DefaultSecondSortOrder);

            this.TrackChanges(vm => vm.DefaultFirstSortDirection, vm => vm.Model.DefaultFirstSortDirection);
            this.TrackChanges(vm => vm.DefaultSecondSortDirection, vm => vm.Model.DefaultSecondSortDirection);

            this.TrackChanges(this.IsCollectionChanged(vm => vm.Kinds, vm => vm.Model.Kinds));

            this.TrackChanges(this.tagsSource.Connect()
                .AutoRefreshOnObservable(tm => tm.WhenAnyPropertyChanged())
                .AutoRefreshOnObservable(tm => tm.ImpliedTags.ToObservableChangeSet())
                .ToCollection()
                .Select(tags => tags.Count != this.Model.Tags.Count || tags.Any(this.TagModelChanged)));

            this.TrackValidation(this.IsCollectionValid(this.Kinds));

            base.EnableChangeTracking();
        }

        protected override IObservable<TSettings> OnSave()
        {
            this.Model.DefaultSeasonTitle = this.DefaultSeasonTitle;
            this.Model.DefaultSeasonOriginalTitle = this.DefaultSeasonOriginalTitle;

            this.Model.CultureInfo = this.CultureInfo;

            this.Model.DefaultFirstSortOrder = this.DefaultFirstSortOrder;
            this.Model.DefaultSecondSortOrder = this.DefaultSecondSortOrder;

            this.Model.DefaultFirstSortDirection = this.DefaultFirstSortDirection;
            this.Model.DefaultSecondSortDirection = this.DefaultSecondSortDirection;

            foreach (var tagModel in this.tagsSource.Items)
            {
                this.SaveTag(tagModel);
            }

            return this.Kinds
                .Select(kindViewModel => kindViewModel.Save.Execute())
                .ForkJoin()
                .Select(_ =>
                {
                    this.Model.Kinds.Clear();
                    this.Model.Kinds.AddRange(this.kindsSource.Items);

                    this.Model.Tags.Clear();
                    this.Model.Tags.AddRange(this.tagsSource.Items.Select(vm => vm.Tag));

                    return this.Model;
                });
        }

        protected override IObservable<TSettings?> OnDelete() =>
            Observable.Return<TSettings?>(null);

        protected override void CopyProperties()
        {
            this.DefaultSeasonTitle = this.Model.DefaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = this.Model.DefaultSeasonOriginalTitle;

            this.CultureInfo = this.Model.CultureInfo;

            this.DefaultFirstSortOrder = this.Model.DefaultFirstSortOrder;
            this.DefaultSecondSortOrder = this.Model.DefaultSecondSortOrder;

            this.DefaultFirstSortDirection = this.Model.DefaultFirstSortDirection;
            this.DefaultSecondSortDirection = this.Model.DefaultSecondSortDirection;

            this.kindsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.Model.Kinds);
            });

            var tagModels = this.CreateTagModels(this.Model.Tags);

            this.tagsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(tagModels);
            });
        }

        protected abstract bool InitialKindIsNewValue(Kind kind);

        protected abstract bool IsTagNew(TagModel tagModel);

        private KindFormViewModel CreateKindForm(Kind kind)
        {
            var isKindNew = new BehaviorSubject<bool>(this.InitialKindIsNewValue(kind));
            var formSubscriptions = new CompositeDisposable();

            var canDeleteKind = this.kindsSource.Connect()
                .Count()
                .Select(count => count > 1);

            var allKindNames = this.Kinds.ToObservableChangeSet()
                .AutoRefresh(kind => kind.Name)
                .ToCollection();

            var form = new KindFormViewModel(
                kind, isKindNew.AsObservable(), canDeleteKind, allKindNames, this.ResourceManager, this.Scheduler);

            form.DisposeWith(formSubscriptions);

            form.Save
                .Select(_ => false)
                .Subscribe(isKindNew)
                .DisposeWith(formSubscriptions);

            form.Delete
                .WhereNotNull()
                .Subscribe(k =>
                {
                    this.kindsSource.Remove(k);
                    formSubscriptions.Dispose();
                })
                .DisposeWith(formSubscriptions);

            return form;
        }

        private List<TagModel> CreateTagModels(List<Tag> tags)
        {
            var tagModelsByTag = tags.ToDictionary(tag => tag, tag => new TagModel(tag));

            foreach (var tm in tagModelsByTag.Values)
            {
                tm.ImpliedTags.AddRange(tm.Tag.ImpliedTags.Select(t => tagModelsByTag[t]));
            }

            return tagModelsByTag.Values.ToList();
        }

        private TagItemViewModel CreateTagItem(TagModel tagModel)
        {
            var vm = new TagItemViewModel(tagModel, canSelect: true);

            var subscriptions = new CompositeDisposable();

            vm.Select
                .Select(_ => vm)
                .InvokeCommand(this.OpenTagForm)
                .DisposeWith(subscriptions);

            vm.Delete
                .SelectMany(_ => Dialog.PromptToDelete("DeleteTag", () => Observable.Return(tagModel)))
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(tm =>
                {
                    this.tagsSource.Remove(tm);
                    subscriptions.Dispose();
                })
                .DisposeWith(subscriptions);

            return vm;
        }

        private IObservable<Unit> OnOpenTagForm(TagItemViewModel tagItem) =>
            Dialog.TagForm.Handle(new TagFormViewModel(tagItem.TagModel!, this.tagsSource.Items))
                .ObserveOn(RxApp.MainThreadScheduler);

        private IObservable<Unit> OnAddTag()
        {
            var tagModel = new TagModel(new Tag());
            var tagForm = new TagFormViewModel(tagModel, this.tagsSource.Items);

            var subscriptions = new CompositeDisposable();

            tagForm.Save
                .Subscribe(_ => this.tagsSource.Add(tagModel))
                .DisposeWith(subscriptions);

            return Dialog.TagForm.Handle(tagForm)
                .Do(_ => subscriptions.Dispose())
                .ObserveOn(RxApp.MainThreadScheduler);
        }

        private bool TagModelChanged(TagModel tagModel) =>
            this.IsTagNew(tagModel) ||
                tagModel.Name != tagModel.Tag.Name ||
                tagModel.Description != tagModel.Tag.Description ||
                tagModel.Category != tagModel.Tag.Category ||
                tagModel.Color != tagModel.Tag.Color ||
                tagModel.IsApplicableToMovies != tagModel.Tag.IsApplicableToMovies ||
                tagModel.IsApplicableToSeries != tagModel.Tag.IsApplicableToSeries ||
                !tagModel.ImpliedTags.Select(tm => tm.Tag).ToHashSet().SetEquals(tagModel.Tag.ImpliedTags);

        private void SaveTag(TagModel tagModel)
        {
            tagModel.Tag.Name = tagModel.Name;
            tagModel.Tag.Description = tagModel.Description;
            tagModel.Tag.Category = tagModel.Category;
            tagModel.Tag.Color = tagModel.Color;
            tagModel.Tag.IsApplicableToMovies = tagModel.IsApplicableToMovies;
            tagModel.Tag.IsApplicableToSeries = tagModel.IsApplicableToSeries;

            tagModel.Tag.ImpliedTags.Clear();
            tagModel.ImpliedTags.Select(tm => tm.Tag).ForEach(t => tagModel.Tag.ImpliedTags.Add(t));
        }
    }
}
