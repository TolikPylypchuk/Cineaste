using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public abstract class FormViewModelBase<TModel, TViewModel> : ReactiveValidationObject<TViewModel>
        where TModel : class
        where TViewModel : FormViewModelBase<TModel, TViewModel>
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

            var canSave = Observable.CombineLatest(this.FormChanged, this.validSubject, this.IsValid()).AllTrue();

            this.Save = ReactiveCommand.CreateFromTask(this.OnSaveAsync, canSave);
            this.Cancel = ReactiveCommand.Create(this.CopyProperties, this.formChangedSubject);
            this.Delete = ReactiveCommand.CreateFromTask(this.OnDeleteAsync, this.canDeleteSubject);
        }

        public IObservable<bool> FormChanged
            => this.formChangedSubject.AsObservable();

        public bool IsFormChanged
            => this.formChangedSubject.Value;

        public abstract bool IsNew { get; }

        public ReactiveCommand<Unit, TModel> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, TModel?> Delete { get; }

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

        protected IObservable<bool> IsCollectionChanged<TVm, TM>(
            Expression<Func<TViewModel, ReadOnlyObservableCollection<TVm>>> property,
            Func<TViewModel, ICollection<TM>> itemCollection)
            where TVm : FormViewModelBase<TM, TVm>
            where TM : class
        {
            string propertyName = property.GetMemberName();

            return property.Compile()(this.Self).ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.FormChanged)
                .ToCollection()
                .Select(vms =>
                    vms.Count == 0 ||
                    vms.Count != itemCollection(this.Self).Count ||
                    vms.Any(vm => vm.IsFormChanged || !this.IsNew && vm.IsNew))
                .Do(changed => this.Log().Debug(
                    changed ? $"{propertyName} are changed" : $"{propertyName} are unchanged"));
        }

        protected IObservable<bool> IsCollectionValid<TVm, TM>(ReadOnlyObservableCollection<TVm> viewModels)
            where TVm : FormViewModelBase<TM, TVm>
            where TM : class
            => viewModels.ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.IsValid())
                .ToCollection()
                .Select(vms => vms.Select(vm => vm.IsValid()).CombineLatest().AllTrue())
                .Switch();

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

        protected abstract Task<TModel> OnSaveAsync();

        protected abstract Task<TModel?> OnDeleteAsync();

        protected abstract void CopyProperties();
    }
}
