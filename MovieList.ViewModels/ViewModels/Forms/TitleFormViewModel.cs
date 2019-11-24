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

namespace MovieList.ViewModels.Forms
{
    public class TitleFormViewModel : ReactiveValidationObject<TitleFormViewModel>
    {
        private readonly BehaviorSubject<bool> formChanged;

        public TitleFormViewModel(Title title, IObservable<bool> canDelete, ResourceManager? resourceManager = null)
        {
            this.Title = title;
            this.Name = title.Name;

            resourceManager ??= Locator.Current.GetService<ResourceManager>();

            this.NameRule = this.ValidationRule(
                vm => vm.Name,
                name => !String.IsNullOrWhiteSpace(name),
                resourceManager.GetString("ValidationTitleNameEmpty"));

            this.formChanged = new BehaviorSubject<bool>(false);

            var canSave = new BehaviorSubject<bool>(false);

            this.Save = ReactiveCommand.Create(this.OnSave, canSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel, this.FormChanged);
            this.Delete = ReactiveCommand.Create(() => { }, canDelete);

            var falseWhenSave = this.Save.Select(_ => false);
            var falseWhenCancel = this.Cancel.Select(_ => false);

            this.WhenAnyValue(vm => vm.Name)
                .Select(name => name != this.Title.Name)
                .Merge(falseWhenSave)
                .Merge(falseWhenCancel)
                .Subscribe(this.formChanged);

            Observable.CombineLatest(this.FormChanged, this.NameRule.Valid())
                .AllTrue()
                .Merge(falseWhenSave)
                .Merge(falseWhenCancel)
                .Subscribe(canSave);
        }

        public Title Title { get; }

        [Reactive]
        public string Name { get; set; }

        public IObservable<bool> FormChanged
            => this.formChanged.AsObservable();

        public bool AreChangesPresent
            => this.formChanged.Value;

        public ValidationHelper NameRule { get; }

        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, Unit> Delete { get; }

        private void OnSave()
        {
            this.Name = this.Name.Trim();
            this.Title.Name = this.Name;
        }

        private void OnCancel()
            => this.Name = this.Title.Name;
    }
}
