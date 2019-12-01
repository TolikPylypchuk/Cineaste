using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class TitleFormViewModel : FormViewModelBase<Title, TitleFormViewModel>
    {
        public TitleFormViewModel(Title title, IObservable<bool> canDelete, ResourceManager? resourceManager = null)
            : base(resourceManager)
        {
            this.Title = title;
            this.CopyProperties();

            this.NameRule = this.ValidationRule(
                vm => vm.Name,
                name => !String.IsNullOrWhiteSpace(name),
                this.ResourceManager.GetString("ValidationTitleNameEmpty"));

            canDelete.Subscribe(this.CanDeleteSubject);

            var canMoveUp = this.WhenAnyValue(vm => vm.Priority)
                .Select(priority => priority >= MinTitleCount);

            this.MoveUp = ReactiveCommand.Create(() => { this.Priority--; }, canMoveUp);

            this.InitializeChangeTracking();
        }

        public Title Title { get; }

        [Reactive]
        public string Name { get; set; } = null!;

        [Reactive]
        public int Priority { get; set; }

        public ValidationHelper NameRule { get; }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }

        protected override void InitializeChangeTracking()
        {
            var nameChanged = this.WhenAnyValue(vm => vm.Name)
                .Select(name => name != this.Title.Name);

            var priorityChanged = this.WhenAnyValue(vm => vm.Priority)
                .Select(priority => priority != this.Title.Priority);

            var falseWhenSave = this.Save.Select(_ => false);
            var falseWhenCancel = this.Cancel.Select(_ => false);

            Observable.CombineLatest(nameChanged, priorityChanged)
                .AnyTrue()
                .Merge(falseWhenSave)
                .Merge(falseWhenCancel)
                .Subscribe(this.FormChangedSubject);

            this.NameRule.Valid()
                .Subscribe(this.ValidSubject);
        }

        protected override Task<Title> OnSaveAsync()
        {
            this.Name = this.Name.Trim().Replace(" - ", " – ");
            this.Title.Name = this.Name;
            this.Title.Priority = this.Priority;

            return Task.FromResult(this.Title);
        }

        [SuppressMessage("ReSharper", "RedundantCast")]
        protected override Task<Title?> OnDeleteAsync()
            => Task.FromResult((Title?)this.Title);

        protected override void CopyProperties()
        {
            this.Name = this.Title.Name;
            this.Priority = this.Title.Priority;
        }
    }
}
