using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using MovieList.Data.Models;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class TitleFormViewModel : ReactiveForm<Title, TitleFormViewModel>
    {
        public TitleFormViewModel(
            Title title,
            IObservable<bool> canDelete,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Title = title;
            this.CopyProperties();

            this.NameRule = this.ValidationRule(
                vm => vm.Name, name => !String.IsNullOrWhiteSpace(name), "TitleNameEmpty");

            this.CanDeleteWhen(canDelete);

            var canMoveUp = this.WhenAnyValue(vm => vm.Priority)
                .Select(priority => priority >= MinTitleCount);

            this.MoveUp = ReactiveCommand.Create(() => { this.Priority--; }, canMoveUp);

            this.EnableChangeTracking();
        }

        public Title Title { get; }

        [Reactive]
        public string Name { get; set; } = null!;

        [Reactive]
        public int Priority { get; set; }

        public ValidationHelper NameRule { get; }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }

        public override bool IsNew
            => this.Title.Id == default;

        protected override TitleFormViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Name, vm => vm.Title.Name);
            this.TrackChanges(vm => vm.Priority, vm => vm.Title.Priority);

            base.EnableChangeTracking();
        }

        protected override IObservable<Title> OnSave()
        {
            this.Name = this.Name.Trim().Replace(" - ", " – ");
            this.Title.Name = this.Name;
            this.Title.Priority = this.Priority;

            return Observable.Return(this.Title);
        }

        protected override IObservable<Title?> OnDelete()
            => Observable.Return(this.Title);

        protected override void CopyProperties()
        {
            this.Name = this.Title.Name;
            this.Priority = this.Title.Priority;
        }
    }
}
