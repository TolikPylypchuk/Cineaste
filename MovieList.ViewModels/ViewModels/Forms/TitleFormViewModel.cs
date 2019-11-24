using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public class TitleFormViewModel : ReactiveValidationObject<TitleFormViewModel>
    {
        private readonly BehaviorSubject<bool> formChanged;

        public TitleFormViewModel(Title title, IObservable<bool> canDelete, ResourceManager? resourceManager = null)
        {
            this.Title = title;
            this.Name = title.Name;
            this.Priority = title.Priority;

            resourceManager ??= Locator.Current.GetService<ResourceManager>();

            this.NameRule = this.ValidationRule(
                vm => vm.Name,
                name => !String.IsNullOrWhiteSpace(name),
                resourceManager.GetString("ValidationTitleNameEmpty"));

            this.formChanged = new BehaviorSubject<bool>(false);

            var canSave = new BehaviorSubject<bool>(false);

            var canMoveUp = this.WhenAnyValue(vm => vm.Priority)
                .Select(priority => priority >= MinTitleCount);

            this.MoveUp = ReactiveCommand.Create(() => { this.Priority--; }, canMoveUp);

            this.Save = ReactiveCommand.Create(this.OnSave, canSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel, this.FormChanged);
            this.Delete = ReactiveCommand.Create(() => { }, canDelete);

            this.InitializeChangeTracking(canSave);
        }

        public Title Title { get; }

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public int Priority { get; set; }

        public IObservable<bool> FormChanged
            => this.formChanged.AsObservable();

        public bool IsFormChanged
            => this.formChanged.Value;

        public ValidationHelper NameRule { get; }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }

        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, Unit> Delete { get; }

        private void InitializeChangeTracking(BehaviorSubject<bool> canSave)
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
                .Subscribe(this.formChanged);

            Observable.CombineLatest(this.FormChanged, this.NameRule.Valid())
                .AllTrue()
                .Merge(falseWhenSave)
                .Merge(falseWhenCancel)
                .Subscribe(canSave);
        }

        private void OnSave()
        {
            this.Name = this.Name.Trim();
            this.Title.Name = this.Name;
            this.Title.Priority = this.Priority;
        }

        private void OnCancel()
        {
            this.Name = this.Title.Name;
            this.Priority = this.Title.Priority;
        }
    }
}
