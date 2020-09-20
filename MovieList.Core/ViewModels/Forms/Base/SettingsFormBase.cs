using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using MovieList.Core.Preferences;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Forms.Base
{
    public abstract class SettingsFormBase<TSettings, TForm> : ReactiveForm<TSettings, TForm>
        where TSettings : class, ISettings
        where TForm : SettingsFormBase<TSettings, TForm>
    {
        private readonly SourceList<Kind> kindsSource = new SourceList<Kind>();
        private readonly ReadOnlyObservableCollection<KindFormViewModel> kinds;

        private readonly SourceList<TagItemViewModel> tagsSource = new SourceList<TagItemViewModel>();
        private readonly ReadOnlyObservableCollection<TagItemViewModel> tags;
        private readonly ReadOnlyObservableCollection<TagFormViewModel> tagForms;

        private readonly Dictionary<Tag, CompositeDisposable> tagItemSubscriptions = new();
        private readonly Dictionary<Tag, CompositeDisposable> tagFormSubscriptions = new();

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
                .Sort(SortExpressionComparer<TagItemViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .AutoRefresh(vm => vm.Category)
                .AutoRefresh(vm => vm.Name)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.tags)
                .DisposeMany()
                .Subscribe();

            this.tags.ToObservableChangeSet()
                .Transform(vm => vm.Form)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.tagForms)
                .DisposeMany()
                .Subscribe();

            this.AddKind = ReactiveCommand.Create(() => this.kindsSource.Add(new Kind()));
            this.AddTag = ReactiveCommand.CreateFromObservable(this.OnAddTag);
            this.OpenTagForm = ReactiveCommand.CreateFromObservable<TagItemViewModel, Unit>(this.OnOpenTagForm);
            this.DeleteTag = ReactiveCommand.Create<Tag>(this.OnDeleteTag);

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

        public ReadOnlyObservableCollection<KindFormViewModel> Kinds
            => this.kinds;

        public ReadOnlyObservableCollection<TagItemViewModel> Tags
            => this.tags;

        public ReadOnlyObservableCollection<TagFormViewModel> TagForms
            => this.tagForms;

        public ReactiveCommand<Unit, Unit> AddKind { get; }
        public ReactiveCommand<Unit, Unit> AddTag { get; }
        public ReactiveCommand<TagItemViewModel, Unit> OpenTagForm { get; }
        public ReactiveCommand<Tag, Unit> DeleteTag { get; }

        public override bool IsNew
            => false;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.DefaultSeasonTitle, vm => vm.Model.DefaultSeasonTitle);
            this.TrackChanges(
                vm => vm.DefaultSeasonOriginalTitle, vm => vm.Model.DefaultSeasonOriginalTitle);
            this.TrackChanges(vm => vm.CultureInfo, vm => vm.Model.CultureInfo);
            this.TrackChanges(this.IsCollectionChanged(vm => vm.Kinds, vm => vm.Model.Kinds));
            this.TrackChanges(this.IsCollectionChanged(vm => vm.TagForms, vm => vm.Model.Tags));
            this.TrackValidation(this.IsCollectionValid(this.Kinds));

            base.EnableChangeTracking();
        }

        protected override IObservable<TSettings> OnSave()
        {
            this.Model.DefaultSeasonTitle = this.DefaultSeasonTitle;
            this.Model.DefaultSeasonOriginalTitle = this.DefaultSeasonOriginalTitle;
            this.Model.CultureInfo = this.CultureInfo;

            var saveKinds = this.Kinds
                .Select(kindViewModel => kindViewModel.Save.Execute())
                .ForkJoin();

            var saveTags = this.TagForms
                .Select(tagViewModel => tagViewModel.Save.Execute())
                .ForkJoin();

            return Observable.CombineLatest(saveKinds, saveTags, (a, b) => Unit.Default)
                .Select(() =>
                {
                    this.Model.Kinds.Clear();
                    this.Model.Kinds.AddRange(this.kindsSource.Items);

                    this.Model.Tags.Clear();
                    this.Model.Tags.AddRange(this.tagsSource.Items.Select(vm => vm.Form.Tag));

                    return this.Model;
                });
        }

        protected override IObservable<TSettings?> OnDelete()
            => Observable.Return<TSettings?>(null);

        protected override void CopyProperties()
        {
            this.DefaultSeasonTitle = this.Model.DefaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = this.Model.DefaultSeasonOriginalTitle;
            this.CultureInfo = this.Model.CultureInfo;

            this.kindsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.Model.Kinds);
            });

            this.tagsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.Model.Tags.Select(this.CreateTagItem));
            });
        }

        protected abstract bool InitialKindIsNewValue(Kind kind);

        private KindFormViewModel CreateKindForm(Kind kind)
        {
            var isKindNew = new BehaviorSubject<bool>(this.InitialKindIsNewValue(kind));
            var formSubscriptions = new CompositeDisposable();

            var canDeleteKind = this.kindsSource.Connect()
                .Count()
                .Select(count => count > 1);

            var form = new KindFormViewModel(
                kind, isKindNew.AsObservable(), canDeleteKind, this.ResourceManager, this.Scheduler);

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

        private TagItemViewModel CreateTagItem(Tag tag)
        {
            var vm = new TagItemViewModel(tag, canSelect: true, canDelete: false);

            var subscriptions = new CompositeDisposable();

            vm.Select
                .Select(_ => vm)
                .InvokeCommand(this.OpenTagForm)
                .DisposeWith(subscriptions);

            this.tagItemSubscriptions.Add(tag, subscriptions);

            return vm;
        }

        private IObservable<Unit> OnOpenTagForm(TagItemViewModel vm)
        {
            var subscriptions = new CompositeDisposable();

            vm.Form.Delete
                .WhereNotNull()
                .InvokeCommand(this.DeleteTag)
                .DisposeWith(subscriptions);

            this.tagFormSubscriptions.Add(vm.Form.Tag, subscriptions);

            return Dialog.TagForm.Handle(vm.Form)
                .Do(_ => this.tagFormSubscriptions.DisposeAndRemove(vm.Form.Tag))
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(saved => saved ? vm.Refresh.Execute() : Observable.Return(Unit.Default));
        }

        private IObservable<Unit> OnAddTag()
        {
            var tag = new Tag();
            var vm = this.CreateTagItem(tag);

            return Dialog.TagForm.Handle(vm.Form)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(saved => saved ? vm.Refresh.Execute().Select(() => saved) : Observable.Return(saved))
                .Do(saved =>
                {
                    if (saved)
                    {
                        this.tagsSource.Add(vm);
                    } else
                    {
                        this.tagItemSubscriptions.DisposeAndRemove(tag);
                    }
                })
                .Discard();
        }

        private void OnDeleteTag(Tag tag)
        {
            this.tagItemSubscriptions.DisposeAndRemove(tag);
            this.tagsSource.Remove(this.tagsSource.Items.First(vm => vm.Form.Tag == tag));
        }
    }
}
