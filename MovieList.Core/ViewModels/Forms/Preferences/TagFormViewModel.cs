using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Resources;

using DynamicData;

using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public sealed class TagFormViewModel : TagFormBase<TagItemViewModel, TagFormViewModel>
    {
        public TagFormViewModel(
            TagItemViewModel tag,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Tag = tag;
            this.CopyProperties();

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

            this.CanNeverDelete();
            this.EnableChangeTracking();
        }

        public TagItemViewModel Tag { get; }

        public string FormTitle { [ObservableAsProperty] get; } = String.Empty;

        public ReactiveCommand<Unit, Unit> Close { get; }

        public ValidationHelper NameRule { get; }
        public ValidationHelper CategoryRule { get; }
        public ValidationHelper ColorRule { get; }

        public override bool IsNew
            => this.Tag.IsNew;

        protected override TagFormViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Name, vm => vm.Tag.Name);
            this.TrackChanges(vm => vm.Description, vm => vm.Tag.Description);
            this.TrackChanges(vm => vm.Category, vm => vm.Tag.Category);
            this.TrackChanges(vm => vm.Color, vm => vm.Tag.Color);

            this.TrackChanges(this.IsCollectionChanged(vm => vm.ImpliedTags, vm => vm.Tag.ImpliedTags));

            base.EnableChangeTracking();
        }

        protected override IObservable<TagItemViewModel> OnSave()
        {
            this.Tag.Name = this.Name = this.Name.EmptyIfNull().Trim();
            this.Tag.Description = this.Description = this.Description.EmptyIfNull().Trim();
            this.Tag.Category = this.Category = this.Category.EmptyIfNull().Trim();
            this.Tag.Color = this.Color = this.Color.EmptyIfNull().Trim();
            this.Tag.IsApplicableToMovies = this.IsApplicableToMovies;
            this.Tag.IsApplicableToSeries = this.IsApplicableToSeries;
            this.Tag.IsApplicableToFranchises = this.IsApplicableToFranchises;

            this.Tag.UpdateImpliedTags(this.ImpliedTagsSource.Items);

            return Observable.Return(this.Tag);
        }

        protected override IObservable<TagItemViewModel?> OnDelete()
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

            this.ImpliedTagsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.Tag.ImpliedTags.Select(vm => vm.Tag));
            });
        }
    }
}
