using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;
using System.Threading.Tasks;

using ReactiveUI;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public abstract class FormViewModelBase<TEntity, TViewModel> : ReactiveValidationObject<TViewModel>
        where TEntity : class
        where TViewModel : FormViewModelBase<TEntity, TViewModel>
    {
        private readonly BehaviorSubject<bool> formChangedSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> validSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> canDeleteSubject = new BehaviorSubject<bool>(false);

        private readonly List<IObservable<bool>> changesToTrack = new List<IObservable<bool>>();
        private readonly List<IObservable<bool>> validationsToTrack = new List<IObservable<bool>>();

        protected FormViewModelBase(ResourceManager? resourceManager = null, IScheduler? scheduler = null)
        {
            this.ResourceManager = resourceManager ?? Locator.Current.GetService<ResourceManager>();
            this.Scheduler = scheduler ?? System.Reactive.Concurrency.Scheduler.Default;

            var canSave = Observable.CombineLatest(this.FormChanged, this.Valid).AllTrue();

            this.Save = ReactiveCommand.CreateFromTask(this.OnSaveAsync, canSave);
            this.Cancel = ReactiveCommand.Create(this.CopyProperties, this.formChangedSubject);
            this.Delete = ReactiveCommand.CreateFromTask(this.OnDeleteAsync, this.canDeleteSubject);
        }

        public IObservable<bool> FormChanged
            => this.formChangedSubject.AsObservable();

        public bool IsFormChanged
            => this.formChangedSubject.Value;

        public IObservable<bool> Valid
            => this.validSubject.AsObservable();

        public bool IsValid
            => this.validSubject.Value;

        public abstract bool IsNew { get; }

        public ReactiveCommand<Unit, TEntity> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, TEntity?> Delete { get; }

        protected ResourceManager ResourceManager { get; }
        protected IScheduler Scheduler { get; }

        protected abstract TViewModel Self { get; }

        protected void TrackChanges(IObservable<bool> changes)
            => this.changesToTrack.Add(changes);

        protected void TrackChanges<T>(Expression<Func<TViewModel, T>> property, Func<TViewModel, T> itemValue)
        {
            string propertyName = property.GetMemberName();

            this.TrackChanges(
                this.Self.WhenAnyValue(property)
                    .Select(value => !Equals(value, itemValue(this.Self)))
                    .Do(changed => this.Log().Debug(
                        changed ? $"{propertyName} is changed" : $"{propertyName} is unchanged")));
        }

        protected void TrackValidation(IObservable<bool> validation)
            => this.validationsToTrack.Add(validation);

        protected void TrackValidation(ValidationHelper rule)
            => this.TrackValidation(rule.Valid());

        protected void CanDeleteWhen(IObservable<bool> canDelete)
            => canDelete.Subscribe(this.canDeleteSubject);

        protected void CanDeleteWhenNotNew()
            => this.CanDeleteWhen(Observable.Return(!this.IsNew).Merge(this.Save.Select(_ => true)));

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

            this.validationsToTrack
                .CombineLatest()
                .AllTrue()
                .Subscribe(this.validSubject);
        }

        protected abstract Task<TEntity> OnSaveAsync();

        protected abstract Task<TEntity?> OnDeleteAsync();

        protected abstract void CopyProperties();
    }
}
