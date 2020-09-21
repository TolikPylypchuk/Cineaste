using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Resources;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public sealed class TagFormViewModel : ReactiveForm<Tag, TagFormViewModel>
    {
        private readonly SourceList<Tag> impliedTagsSource = new SourceList<Tag>();
        private readonly ReadOnlyObservableCollection<TagItemViewModel> impliedTags;

        public TagFormViewModel(Tag tag, ResourceManager? resourceManager = null, IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Tag = tag;
            this.CopyProperties();

            this.impliedTagsSource.Connect()
                .Transform(this.CreateTagItemViewModel)
                .Sort(SortExpressionComparer<TagItemViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .Bind(out this.impliedTags)
                .DisposeMany()
                .Subscribe();

            static bool notEmpty(string str) => !String.IsNullOrWhiteSpace(str);

            this.Close = ReactiveCommand.Create(() => { });

            this.NameRule = this.ValidationRule(vm => vm.Name, notEmpty, "NameEmpty");
            this.CategoryRule = this.ValidationRule(vm => vm.Category, notEmpty, "CategoryEmpty");
            this.ColorRule = this.ValidationRuleForColor(vm => vm.Color);

            this.WhenAnyValue(vm => vm.Name)
                .Select(name => this.IsNew && String.IsNullOrWhiteSpace(name)
                    ? this.ResourceManager.GetString("NewTag") ?? String.Empty
                    : name)
                .ToPropertyEx(this, vm => vm.FormTitle);

            this.CanAlwaysDelete();
            this.EnableChangeTracking();
        }

        public Tag Tag { get; }

        public string FormTitle { [ObservableAsProperty] get; } = String.Empty;

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

        [Reactive]
        public bool IsApplicableToFranchises { get; set; }

        public ReadOnlyObservableCollection<TagItemViewModel> ImpliedTags
            => this.impliedTags;

        public ReactiveCommand<Unit, Unit> Close { get; }

        public ValidationHelper NameRule { get; }
        public ValidationHelper CategoryRule { get; }
        public ValidationHelper ColorRule { get; }

        public override bool IsNew
            => this.Tag.Id == default;

        protected override TagFormViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Name, vm => vm.Tag.Name);
            this.TrackChanges(vm => vm.Description, vm => vm.Tag.Description);
            this.TrackChanges(vm => vm.Category, vm => vm.Tag.Category);
            this.TrackChanges(vm => vm.Color, vm => vm.Tag.Color);

            var impliedTagsChanged = this.impliedTagsSource.Connect()
                .ToCollection()
                .Select(tags => tags
                    .OrderBy(t => t.Category)
                    .ThenBy(t => t.Name)
                    .SequenceEqual(this.Tag.ImpliedTags.OrderBy(t => t.Category).ThenBy(t => t.Name)))
                .Invert();

            this.TrackChanges(impliedTagsChanged);

            base.EnableChangeTracking();
        }

        protected override IObservable<Tag> OnSave()
        {
            this.Tag.Name = this.Name = this.Name.EmptyIfNull().Trim();
            this.Tag.Description = this.Description = this.Description.EmptyIfNull().Trim();
            this.Tag.Category = this.Category = this.Category.EmptyIfNull().Trim();
            this.Tag.Color = this.Color = this.Color.EmptyIfNull().Trim();
            this.Tag.IsApplicableToMovies = this.IsApplicableToMovies;
            this.Tag.IsApplicableToSeries = this.IsApplicableToSeries;
            this.Tag.IsApplicableToFranchises = this.IsApplicableToFranchises;

            this.Tag.ImpliedTags.Clear();
            this.impliedTagsSource.Items.ForEach(tag => this.Tag.ImpliedTags.Add(tag));

            return Observable.Return(this.Tag);
        }

        protected override IObservable<Tag?> OnDelete()
            => Observable.Return(this.Tag);

        protected override void CopyProperties()
        {
            this.Name = this.Tag.Name;
            this.Description = this.Tag.Description;
            this.Category = this.Tag.Category;
            this.Color = this.Tag.Color;
            this.IsApplicableToMovies = this.Tag.IsApplicableToMovies;
            this.IsApplicableToSeries = this.Tag.IsApplicableToSeries;
            this.IsApplicableToFranchises = this.Tag.IsApplicableToFranchises;

            this.impliedTagsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.Tag.ImpliedTags);
            });
        }

        private TagItemViewModel CreateTagItemViewModel(Tag tag)
        {
            var viewModel = new TagItemViewModel(tag, canSelect: false, canDelete: true);

            var subscriptions = new CompositeDisposable();

            viewModel.Delete
                .Subscribe(() =>
                {
                    this.impliedTagsSource.Remove(tag);
                    subscriptions.Dispose();
                })
                .DisposeWith(subscriptions);

            return viewModel;
        }
    }
}
