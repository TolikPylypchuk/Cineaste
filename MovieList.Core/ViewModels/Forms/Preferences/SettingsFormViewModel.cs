using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.Models;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms.Preferences
{
    public sealed class SettingsFormViewModel : ReactiveForm<SettingsModel, SettingsFormViewModel>
    {
        private readonly ISettingsService settingsService;
        private readonly IKindService kindService;

        private readonly SourceList<Kind> kindsSource = new SourceList<Kind>();
        private readonly ReadOnlyObservableCollection<KindFormViewModel> kinds;

        public SettingsFormViewModel(
            string fileName,
            Settings settings,
            IEnumerable<Kind> kinds,
            ISettingsService? settingsService = null,
            IKindService? kindService = null,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.SettingsModel = new SettingsModel(settings, kinds.ToList());

            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>(fileName);
            this.kindService = kindService ?? Locator.Current.GetService<IKindService>(fileName);

            this.kindsSource.Connect()
                .Transform(this.CreateKindForm)
                .Bind(out this.kinds)
                .Subscribe();

            this.ListNameRule = this.ValidationRule(
                vm => vm.ListName, name => !String.IsNullOrWhiteSpace(name), "ListNameEmpty");

            this.AddKind = ReactiveCommand.Create(() => this.kindsSource.Add(new Kind()));

            this.CopyProperties();
            this.CanNeverDelete();
            this.EnableChangeTracking();
        }

        public SettingsModel SettingsModel { get; }

        [Reactive]
        public string ListName { get; set; } = String.Empty;

        [Reactive]
        public string DefaultSeasonTitle { get; set; } = String.Empty;

        [Reactive]
        public string DefaultSeasonOriginalTitle { get; set; } = String.Empty;

        [Reactive]
        public CultureInfo CultureInfo { get; set; } = null!;

        public ReadOnlyObservableCollection<KindFormViewModel> Kinds
            => this.kinds;

        public ValidationHelper ListNameRule { get; }

        public ReactiveCommand<Unit, Unit> AddKind { get; }

        public override bool IsNew
            => false;

        protected override SettingsFormViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.ListName, vm => vm.SettingsModel.Settings.ListName);
            this.TrackChanges(vm => vm.DefaultSeasonTitle, vm => vm.SettingsModel.Settings.DefaultSeasonTitle);
            this.TrackChanges(
                vm => vm.DefaultSeasonOriginalTitle, vm => vm.SettingsModel.Settings.DefaultSeasonOriginalTitle);
            this.TrackChanges(vm => vm.CultureInfo, vm => vm.SettingsModel.Settings.CultureInfo);
            this.TrackChanges(this.IsCollectionChanged(vm => vm.Kinds, vm => vm.SettingsModel.Kinds));
            this.TrackValidation(this.IsCollectionValid(this.Kinds));

            base.EnableChangeTracking();
        }

        protected override async Task<SettingsModel> OnSaveAsync()
        {
            this.SettingsModel.Settings.ListName = this.ListName;
            this.SettingsModel.Settings.DefaultSeasonTitle = this.DefaultSeasonTitle;
            this.SettingsModel.Settings.DefaultSeasonOriginalTitle = this.DefaultSeasonOriginalTitle;
            this.SettingsModel.Settings.CultureInfo = this.CultureInfo;

            foreach (var kindViewModel in this.Kinds)
            {
                await kindViewModel.Save.Execute();
            }

            this.SettingsModel.Kinds.Clear();
            this.SettingsModel.Kinds.AddRange(this.kindsSource.Items);

            this.settingsService.UpdateSettings(this.SettingsModel.Settings);
            this.kindService.UpdateKinds(this.SettingsModel.Kinds);

            return this.SettingsModel;
        }

        protected override Task<SettingsModel?> OnDeleteAsync()
            => Task.FromResult<SettingsModel?>(null);

        protected override void CopyProperties()
        {
            this.ListName = this.SettingsModel.Settings.ListName;
            this.DefaultSeasonTitle = this.SettingsModel.Settings.DefaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = this.SettingsModel.Settings.DefaultSeasonOriginalTitle;
            this.CultureInfo = this.SettingsModel.Settings.CultureInfo;

            this.kindsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(this.SettingsModel.Kinds);
            });
        }

        private KindFormViewModel CreateKindForm(Kind kind)
        {
            var form = new KindFormViewModel(kind, this.ResourceManager, this.Scheduler);

            form.Delete
                .WhereNotNull()
                .Subscribe(k => this.kindsSource.Remove(k));

            return form;
        }
    }
}
