using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public class TagItemViewModel : TagFormBase<Tag, TagItemViewModel>
    {
        public TagItemViewModel(
            Tag tag,
            bool canSelect,
            ResourceManager? resourceManager = null, IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Tag = tag;
            this.CanSelect = canSelect;
            this.CopyProperties();

            this.Select = ReactiveCommand.Create(() => { }, Observable.Return(canSelect));

            this.CanAlwaysDelete();
            this.EnableChangeTracking();
        }

        public Tag Tag { get; }

        public bool CanSelect { get; }

        public ReactiveCommand<Unit, Unit> Select { get; }

        public override bool IsNew
            => this.Tag.Id == default;

        protected override TagItemViewModel Self
            => this;

        public void UpdateImpliedTags(IEnumerable<Tag> tags)
            => this.ImpliedTagsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(tags);
            });

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Name, vm => vm.Tag.Name);
            this.TrackChanges(vm => vm.Description, vm => vm.Tag.Description);
            this.TrackChanges(vm => vm.Category, vm => vm.Tag.Category);
            this.TrackChanges(vm => vm.Color, vm => vm.Tag.Color);

            this.TrackChanges(this.IsCollectionChanged(vm => vm.ImpliedTags, vm => vm.Tag.ImpliedTags));

            base.EnableChangeTracking();
        }

        protected override IObservable<Tag> OnSave()
        {
            this.Tag.Name = this.Name;
            this.Tag.Category = this.Category;
            this.Tag.Description = this.Description;
            this.Tag.Color = this.Color;
            this.Tag.IsApplicableToMovies = this.IsApplicableToMovies;
            this.Tag.IsApplicableToSeries = this.IsApplicableToSeries;
            this.Tag.IsApplicableToFranchises = this.IsApplicableToFranchises;

            return Observable.Return(this.Tag);
        }

        protected override IObservable<Tag?> OnDelete()
            => Observable.Return(this.Tag);

        protected override void CopyProperties()
        {
            this.Name = this.Tag.Name;
            this.Category = this.Tag.Category;
            this.Description = this.Tag.Description;
            this.Color = this.Tag.Color;
            this.IsApplicableToMovies = this.Tag.IsApplicableToMovies;
            this.IsApplicableToSeries = this.Tag.IsApplicableToSeries;
            this.IsApplicableToFranchises = this.Tag.IsApplicableToFranchises;
        }
    }
}
