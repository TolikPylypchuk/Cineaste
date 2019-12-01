using System;
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
    {
        protected readonly ResourceManager ResourceManager;
        protected readonly IScheduler Scheduler;

        protected readonly BehaviorSubject<bool> FormChangedSubject = new BehaviorSubject<bool>(false);
        protected readonly BehaviorSubject<bool> ValidSubject = new BehaviorSubject<bool>(false);
        protected readonly BehaviorSubject<bool> CanDeleteSubject = new BehaviorSubject<bool>(false);

        protected FormViewModelBase(ResourceManager? resourceManager = null, IScheduler? scheduler = null)
        {
            this.ResourceManager = resourceManager ?? Locator.Current.GetService<ResourceManager>();
            this.Scheduler = scheduler ?? System.Reactive.Concurrency.Scheduler.Default;

            var canSave = Observable.CombineLatest(this.FormChanged, this.Valid).AllTrue();

            this.Save = ReactiveCommand.CreateFromTask(this.OnSaveAsync, canSave);
            this.Cancel = ReactiveCommand.Create(this.CopyProperties, this.FormChangedSubject);
            this.Delete = ReactiveCommand.CreateFromTask(this.OnDeleteAsync, this.CanDeleteSubject);
        }

        public IObservable<bool> FormChanged
            => this.FormChangedSubject.AsObservable();

        public bool IsFormChanged
            => this.FormChangedSubject.Value;

        public IObservable<bool> Valid
            => this.ValidSubject.AsObservable();

        public bool IsValid
            => this.ValidSubject.Value;

        public ReactiveCommand<Unit, TEntity> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, TEntity?> Delete { get; }

        protected abstract void InitializeChangeTracking();

        protected abstract Task<TEntity> OnSaveAsync();

        protected abstract Task<TEntity?> OnDeleteAsync();

        protected abstract void CopyProperties();
    }
}
