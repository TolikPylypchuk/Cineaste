using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.Models;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms.Preferences
{
    public sealed class SettingsFormViewModel : SettingsFormBase<SettingsModel, SettingsFormViewModel>
    {
        private readonly ISettingsService settingsService;
        private readonly IKindService kindService;

        public SettingsFormViewModel(
            string fileName,
            Settings settings,
            IEnumerable<Kind> kinds,
            ISettingsService? settingsService = null,
            IKindService? kindService = null,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(new SettingsModel(settings, kinds.ToList()), resourceManager, scheduler)
        {
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>(fileName);
            this.kindService = kindService ?? Locator.Current.GetService<IKindService>(fileName);

            this.ListNameRule = this.ValidationRule(
                vm => vm.ListName, name => !String.IsNullOrWhiteSpace(name), "ListNameEmpty");

            this.CopyProperties();
            this.CanNeverDelete();
            this.EnableChangeTracking();
        }

        [Reactive]
        public string ListName { get; set; } = String.Empty;

        public ValidationHelper ListNameRule { get; }

        protected override SettingsFormViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.ListName, vm => vm.Model.Settings.ListName);
            base.EnableChangeTracking();
        }

        protected override IObservable<SettingsModel> OnSave()
        {
            this.Model.Settings.ListName = this.ListName;

            return base.OnSave()
                .DoAsync(_ => this.settingsService.UpdateSettingsInTaskPool(this.Model.Settings))
                .DoAsync(_ => this.kindService.UpdateKindsInTaskPool(this.Model.Kinds));
        }

        protected override IObservable<SettingsModel?> OnDelete()
            => Observable.Return<SettingsModel?>(null);

        protected override void CopyProperties()
        {
            this.ListName = this.Model.Settings.ListName;
            base.CopyProperties();
        }

        protected override bool InitialKindIsNewValue(Kind kind)
            => kind.Id == default;
    }
}
