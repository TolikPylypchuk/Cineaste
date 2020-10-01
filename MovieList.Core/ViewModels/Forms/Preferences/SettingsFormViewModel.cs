using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using MovieList.Core.Data.Services;
using MovieList.Core.Models;
using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;

using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public sealed class SettingsFormViewModel : SettingsFormBase<SettingsModel, SettingsFormViewModel>
    {
        private readonly ISettingsService settingsService;
        private readonly ISettingsEntityService<Kind> kindService;
        private readonly ISettingsEntityService<Tag> tagService;

        public SettingsFormViewModel(
            string fileName,
            Settings settings,
            IEnumerable<Kind> kinds,
            IEnumerable<Tag> tags,
            ISettingsService? settingsService = null,
            ISettingsEntityService<Kind>? kindService = null,
            ISettingsEntityService<Tag>? tagService = null,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(new SettingsModel(settings, kinds.ToList(), tags.ToList()), resourceManager, scheduler)
        {
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>(fileName);
            this.kindService = kindService ?? Locator.Current.GetService<ISettingsEntityService<Kind>>(fileName);
            this.tagService = tagService ?? Locator.Current.GetService<ISettingsEntityService<Tag>>(fileName);

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
                .DoAsync(_ => this.kindService.UpdateAllInTaskPool(this.Model.Kinds))
                .DoAsync(_ => this.tagService.UpdateAllInTaskPool(this.Model.Tags));
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

        protected override bool IsTagNew(TagModel tagModel)
            => tagModel.Tag.Id == default;
    }
}
