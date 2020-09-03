using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

using DynamicData;
using DynamicData.Aggregation;

using MovieList.Core.Preferences;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Forms.Base
{
    public abstract class SettingsFormBase<TSettings, TForm> : ReactiveForm<TSettings, TForm>
        where TSettings : class, ISettings
        where TForm : SettingsFormBase<TSettings, TForm>
    {
        private readonly SourceList<Kind> kindsSource = new SourceList<Kind>();
        private readonly ReadOnlyObservableCollection<KindFormViewModel> kinds;

        protected SettingsFormBase(
            TSettings settings,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Model = settings;

            this.kindsSource.Connect()
                .Transform(this.CreateKindForm)
                .Bind(out this.kinds)
                .Subscribe();

            this.AddKind = ReactiveCommand.Create(() => this.kindsSource.Add(new Kind()));

            this.DefaultSeasonTitle = String.Empty;
            this.DefaultSeasonOriginalTitle = String.Empty;
            this.CultureInfo = null!;
        }

        public TSettings Model { get; }

        [Reactive]
        public string DefaultSeasonTitle { get; set; }

        [Reactive]
        public string DefaultSeasonOriginalTitle { get; set; }

        [Reactive]
        public CultureInfo CultureInfo { get; set; }

        public ReadOnlyObservableCollection<KindFormViewModel> Kinds
            => this.kinds;

        public ReactiveCommand<Unit, Unit> AddKind { get; }

        public override bool IsNew
            => false;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.DefaultSeasonTitle, vm => vm.Model.DefaultSeasonTitle);
            this.TrackChanges(
                vm => vm.DefaultSeasonOriginalTitle, vm => vm.Model.DefaultSeasonOriginalTitle);
            this.TrackChanges(vm => vm.CultureInfo, vm => vm.Model.CultureInfo);
            this.TrackChanges(this.IsCollectionChanged(vm => vm.Kinds, vm => vm.Model.Kinds));
            this.TrackValidation(this.IsCollectionValid(this.Kinds));

            base.EnableChangeTracking();
        }

        protected override IObservable<TSettings> OnSave()
        {
            this.Model.DefaultSeasonTitle = this.DefaultSeasonTitle;
            this.Model.DefaultSeasonOriginalTitle = this.DefaultSeasonOriginalTitle;
            this.Model.CultureInfo = this.CultureInfo;

            return this.Kinds
                .Select(kindViewModel => kindViewModel.Save.Execute())
                .ForkJoin()
                .Discard()
                .Select(() =>
                {
                    this.Model.Kinds.Clear();
                    this.Model.Kinds.AddRange(this.kindsSource.Items);

                    return this.Model;
                });
        }

        protected override IObservable<TSettings?> OnDelete()
            => Observable.Return<TSettings?>(null);

        protected override void CopyProperties()
        {
            this.DefaultSeasonTitle = this.Model.DefaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = this.Model.DefaultSeasonOriginalTitle;
            this.CultureInfo = this.Model.CultureInfo;

            this.kindsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.Model.Kinds);
            });
        }

        protected abstract bool InitialKindIsNewValue(Kind kind);

        private KindFormViewModel CreateKindForm(Kind kind)
        {
            var isKindNew = new BehaviorSubject<bool>(this.InitialKindIsNewValue(kind));
            var formSubscriptions = new CompositeDisposable();

            var canDeleteKind = this.kindsSource.Connect()
                .Count()
                .Select(count => count > 1);

            var form = new KindFormViewModel(
                kind, isKindNew.AsObservable(), canDeleteKind, this.ResourceManager, this.Scheduler);

            form.Save
                .Select(_ => false)
                .Subscribe(isKindNew)
                .DisposeWith(formSubscriptions);

            form.Delete
                .WhereNotNull()
                .Subscribe(k =>
                {
                    this.kindsSource.Remove(k);
                    formSubscriptions.Dispose();
                })
                .DisposeWith(formSubscriptions);

            return form;
        }
    }
}
