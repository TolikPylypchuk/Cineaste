using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using DynamicData;
using DynamicData.Binding;

using MovieList.DialogModels;

using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms.Base
{
    public abstract class ReactiveForm<TModel, TForm> : ReactiveValidationObject<TForm>, IReactiveForm
        where TModel : class
        where TForm : ReactiveForm<TModel, TForm>
    {
        private readonly BehaviorSubject<bool> formChangedSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> validSubject = new BehaviorSubject<bool>(true);
        private readonly BehaviorSubject<bool> canSaveSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> canDeleteSubject = new BehaviorSubject<bool>(false);

        private readonly List<IObservable<bool>> changesToTrack = new List<IObservable<bool>>();
        private readonly List<IObservable<bool>> validationsToTrack = new List<IObservable<bool>>();

        protected ReactiveForm(ResourceManager? resourceManager = null, IScheduler? scheduler = null)
        {
            this.ResourceManager = resourceManager ?? Locator.Current.GetService<ResourceManager>();
            this.Scheduler = scheduler ?? System.Reactive.Concurrency.Scheduler.Default;

            this.Valid = Observable.CombineLatest(this.validSubject, this.IsValid()).AllTrue();

            var canSave = Observable.CombineLatest(
                    Observable.CombineLatest(this.formChangedSubject, this.canSaveSubject).AnyTrue(),
                    this.Valid)
                .AllTrue();

            this.Save = ReactiveCommand.CreateFromObservable(this.OnSave, canSave);
            this.Cancel = ReactiveCommand.Create(this.CopyProperties, this.formChangedSubject);
            this.Delete = ReactiveCommand.CreateFromObservable(this.OnDelete, this.canDeleteSubject);
        }

        public IObservable<bool> FormChanged
            => this.formChangedSubject.AsObservable();

        public bool IsFormChanged
            => this.formChangedSubject.Value;

        public IObservable<bool> Valid { get; }

        public abstract bool IsNew { get; }

        public ReactiveCommand<Unit, TModel> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, TModel?> Delete { get; }

        protected ResourceManager ResourceManager { get; }
        protected IScheduler Scheduler { get; }

        protected abstract TForm Self { get; }

        public override IEnumerable GetErrors(string propertyName)
            => this.IsFormChanged ? base.GetErrors(propertyName) : Enumerable.Empty<string>();

        protected void TrackChanges(IObservable<bool> changes)
            => this.changesToTrack.Add(changes.StartWith(false));

        protected void TrackChanges<T>(Expression<Func<TForm, T>> property, Func<TForm, T> itemValue)
        {
            string propertyName = property.GetMemberName();

            this.TrackChanges(
                this.Self.WhenAnyValue(property)
                    .Select(value => !Equals(value, itemValue(this.Self)))
                    .Do(changed => this.Log().Debug(
                        changed ? $"{propertyName} is changed" : $"{propertyName} is unchanged")));
        }

        protected void TrackValidation(IObservable<bool> validation)
            => this.validationsToTrack.Add(validation.StartWith(true));

        protected void TrackValidationStrict(IObservable<bool> validation)
            => this.validationsToTrack.Add(validation.StartWith(false));

        protected IObservable<bool> IsCollectionChanged<TOtherForm, TOtherModel>(
            Expression<Func<TForm, ReadOnlyObservableCollection<TOtherForm>>> property,
            Func<TForm, ICollection<TOtherModel>> itemCollection)
            where TOtherForm : ReactiveForm<TOtherModel, TOtherForm>
            where TOtherModel : class
        {
            string propertyName = property.GetMemberName();

            return property.Compile()(this.Self)
                .ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.FormChanged)
                .ToCollection()
                .Select(vms =>
                    vms.Count != itemCollection(this.Self).Count ||
                    vms.Any(vm => vm.IsFormChanged || !this.IsNew && vm.IsNew))
                .Do(changed => this.Log().Debug(
                    changed ? $"{propertyName} are changed" : $"{propertyName} are unchanged"));
        }

        protected IObservable<bool> IsCollectionValid<TOtherForm>(ReadOnlyObservableCollection<TOtherForm> viewModels)
            where TOtherForm : IReactiveForm
            => viewModels.ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.Valid)
                .ToCollection()
                .Select(vms => vms.Select(vm => vm.Valid).CombineLatest().AllTrue())
                .Switch();

        protected void CanDeleteWhen(IObservable<bool> canDelete)
            => canDelete.Subscribe(this.canDeleteSubject);

        protected void CanDeleteWhenNotChanged()
            => this.CanDeleteWhen(Observable.Return(!this.IsNew).Merge(this.FormChanged.Invert()));

        protected void CanAlwaysDelete()
            => this.CanDeleteWhen(Observable.Return(true));

        protected void CanNeverDelete()
            => this.CanDeleteWhen(Observable.Return(false));

        protected virtual void EnableChangeTracking()
        {
            var falseWhenSave = this.Save.Select(_ => false);
            var falseWhenCancel = this.Cancel.Select(_ => false);

            this.changesToTrack
                .CombineLatest()
                .AnyTrue()
                .Merge(falseWhenSave)
                .Merge(falseWhenCancel)
                .Subscribe(this.formChangedSubject);

            Observable.Return(this.IsNew)
                .Merge(falseWhenSave)
                .Subscribe(this.canSaveSubject);

            this.validationsToTrack
                .CombineLatest()
                .AllTrue()
                .Subscribe(this.validSubject);
        }

        protected IObservable<TModel?> PromptToDelete(string messageAndTitle, Func<IObservable<TModel>> onDelete)
            => Dialog.Confirm.Handle(new ConfirmationModel(messageAndTitle))
                .SelectMany(shouldDelete => shouldDelete ? onDelete() : Observable.Return<TModel?>(null));

        protected abstract IObservable<TModel> OnSave();

        protected abstract IObservable<TModel?> OnDelete();

        protected abstract void CopyProperties();
    }
}
