using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Resources;

using Cineaste.Core.Models;
using Cineaste.Core.ViewModels.Forms.Base;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace Cineaste.Core.ViewModels.Forms.Preferences
{
    public sealed class TagFormViewModel : ReactiveForm<TagModel, TagFormViewModel>
    {
        private readonly SourceList<TagModel> impliedTagsSource = new();
        private readonly SourceList<TagModel> addableImpliedTagsSource = new();

        private readonly ReadOnlyObservableCollection<TagItemViewModel> impliedTags;
        private readonly ReadOnlyObservableCollection<AddableTagViewModel> addableImpliedTags;

        public TagFormViewModel(
            TagModel tagModel,
            IEnumerable<TagModel> allTags,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.TagModel = tagModel;
            this.CopyProperties();

            this.impliedTagsSource.Connect()
                .Transform(this.CreateTagItemViewModel)
                .Sort(SortExpressionComparer<TagItemViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .AutoRefresh(vm => vm.Name)
                .AutoRefresh(vm => vm.Category)
                .Bind(out this.impliedTags)
                .DisposeMany()
                .Subscribe();

            this.addableImpliedTagsSource.Connect()
                .Transform(tag => new AddableTagViewModel(tag))
                .Sort(SortExpressionComparer<AddableTagViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .AutoRefresh(vm => vm.Name)
                .AutoRefresh(vm => vm.Category)
                .Bind(out this.addableImpliedTags)
                .DisposeMany()
                .Subscribe();

            var allTagsList = allTags.ToList();

            this.addableImpliedTagsSource.AddRange(allTagsList
                .Where(tag => !tag.GetImpliedTagsClosure().Contains(tagModel)));

            this.impliedTagsSource.Connect()
                .ActOnEveryObject(
                    onAdd: t => this.addableImpliedTagsSource.Remove(t),
                    onRemove: this.addableImpliedTagsSource.Add);

            static bool notEmpty(string str) => !String.IsNullOrWhiteSpace(str);

            this.AddImpliedTag = ReactiveCommand.Create<TagModel>(this.impliedTagsSource.Add);
            this.Close = ReactiveCommand.Create(() => { });

            this.ValidationRule(vm => vm.Name, notEmpty, "NameEmpty");
            this.ValidationRule(vm => vm.Category, notEmpty, "CategoryEmpty");
            this.ValidationRuleForColor(vm => vm.Color);

            var nameAndCategoryObservable = this.WhenAnyValue(
                vm => vm.Name,
                vm => vm.Category,
                (name, category) => (Name: name, Category: category));

            this.UniqueRule = this.ValidationRule(
                nameAndCategoryObservable,
                nameAndCategory => !allTagsList.Any(tag =>
                    tag != this.TagModel &&
                    tag.Name == nameAndCategory.Name &&
                    tag.Category == nameAndCategory.Category),
                nameAndCategory => String.Format(
                    this.ResourceManager.GetString("ValidationTagNotUniqueFormat") ?? String.Empty,
                    nameAndCategory.Name,
                    nameAndCategory.Category));

            this.WhenAnyValue(vm => vm.Name)
                .Select(name => this.IsNew && String.IsNullOrWhiteSpace(name)
                    ? this.ResourceManager.GetString("NewTag") ?? String.Empty
                    : name)
                .ToPropertyEx(this, vm => vm.FormTitle);

            this.CanNeverDelete();
            this.EnableChangeTracking();
        }

        public TagModel TagModel { get; }

        [Reactive]
        public string Name { get; set; } = String.Empty;

        [Reactive]
        public string Description { get; set; } = String.Empty;

        [Reactive]
        public string Category { get; set; } = String.Empty;

        [Reactive]
        public string Color { get; set; } = String.Empty;

        [Reactive]
        public bool IsApplicableToMovies { get; set; }

        [Reactive]
        public bool IsApplicableToSeries { get; set; }

        public ReadOnlyObservableCollection<TagItemViewModel> ImpliedTags =>
            this.impliedTags;

        public ReadOnlyObservableCollection<AddableTagViewModel> AddableImpliedTags =>
            this.addableImpliedTags;

        public string FormTitle { [ObservableAsProperty] get; } = String.Empty;

        public ValidationHelper UniqueRule { get; }

        public ReactiveCommand<TagModel, Unit> AddImpliedTag { get; }
        public ReactiveCommand<Unit, Unit> Close { get; }

        public override bool IsNew =>
            this.TagModel.Tag.Id == default;

        protected override TagFormViewModel Self => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Name, vm => vm.TagModel.Name);
            this.TrackChanges(vm => vm.Description, vm => vm.TagModel.Description);
            this.TrackChanges(vm => vm.Category, vm => vm.TagModel.Category);
            this.TrackChanges(vm => vm.Color, vm => vm.TagModel.Color);
            this.TrackChanges(vm => vm.IsApplicableToMovies, vm => vm.TagModel.IsApplicableToMovies);
            this.TrackChanges(vm => vm.IsApplicableToSeries, vm => vm.TagModel.IsApplicableToSeries);

            this.TrackChanges(this.impliedTagsSource.Connect()
                .ToCollection()
                .Select(impliedTags => !new HashSet<TagModel>(impliedTags).SetEquals(this.TagModel.ImpliedTags)));

            base.EnableChangeTracking();
        }

        protected override IObservable<TagModel> OnSave()
        {
            this.TagModel.Name = this.Name = this.Name.EmptyIfNull().Trim();
            this.TagModel.Description = this.Description = this.Description.EmptyIfNull().Trim();
            this.TagModel.Category = this.Category = this.Category.EmptyIfNull().Trim();
            this.TagModel.Color = this.Color = this.Color.EmptyIfNull().Trim();
            this.TagModel.IsApplicableToMovies = this.IsApplicableToMovies;
            this.TagModel.IsApplicableToSeries = this.IsApplicableToSeries;

            this.TagModel.ImpliedTags.Clear();
            this.TagModel.ImpliedTags.AddRange(this.impliedTagsSource.Items);

            return Observable.Return(this.TagModel);
        }

        protected override IObservable<TagModel?> OnDelete() =>
            Observable.Return(this.TagModel);

        protected override void CopyProperties()
        {
            this.Name = this.TagModel.Name;
            this.Description = this.TagModel.Description;
            this.Category = this.TagModel.Category;
            this.Color = this.TagModel.Color;
            this.IsApplicableToMovies = this.TagModel.IsApplicableToMovies;
            this.IsApplicableToSeries = this.TagModel.IsApplicableToSeries;

            this.impliedTagsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.TagModel.ImpliedTags);
            });
        }

        private TagItemViewModel CreateTagItemViewModel(TagModel tagModel)
        {
            var vm = new TagItemViewModel(tagModel, canSelect: false);

            var subscriptions = new CompositeDisposable();

            vm.Delete
                .Subscribe(_ =>
                {
                    this.impliedTagsSource.Remove(tagModel);
                    subscriptions.Dispose();
                })
                .DisposeWith(subscriptions);

            return vm;
        }
    }
}
