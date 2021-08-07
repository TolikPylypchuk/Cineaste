using System;
using System.Reactive.Concurrency;
using System.Resources;

using Akavache;

using Cineaste.Core.Models;
using Cineaste.Core.Preferences;
using Cineaste.Core.Theming;
using Cineaste.Core.ViewModels.Forms.Base;
using Cineaste.Data.Models;

using ReactiveUI.Fody.Helpers;

using static Cineaste.Core.Constants;
using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Core.ViewModels.Forms.Preferences
{
    public sealed class PreferencesFormViewModel : SettingsFormBase<UserPreferences, PreferencesFormViewModel>
    {
        private readonly IBlobCache store;
        private readonly ThemeManager themeManager;

        public PreferencesFormViewModel(
            UserPreferences userPreferences,
            IBlobCache? store = null,
            ThemeManager? themeManager = null,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(userPreferences, resourceManager, scheduler)
        {
            this.store = store ?? GetDefaultService<IBlobCache>(StoreKey);
            this.themeManager = themeManager ?? GetDefaultService<ThemeManager>();

            this.Header = new TabHeaderViewModel(
                String.Empty, this.ResourceManager.GetString("Preferences") ?? String.Empty);

            this.CopyProperties();
            this.CanNeverDelete();
            this.EnableChangeTracking();
        }

        [Reactive]
        public Theme Theme { get; set; }

        [Reactive]
        public bool ShowRecentFiles { get; set; }

        [Reactive]
        public string LogPath { get; set; } = String.Empty;

        [Reactive]
        public int MinLogLevel { get; set; }

        public TabHeaderViewModel Header { get; }

        protected override PreferencesFormViewModel Self => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.Theme, vm => vm.Model.UI.Theme);
            this.TrackChanges(vm => vm.ShowRecentFiles, vm => vm.Model.File.ShowRecentFiles);
            this.TrackChanges(vm => vm.LogPath, vm => vm.Model.Logging.LogPath);
            this.TrackChanges(vm => vm.MinLogLevel, vm => vm.Model.Logging.MinLogLevel);

            base.EnableChangeTracking();
        }

        protected override IObservable<UserPreferences> OnSave()
        {
            this.Model.UI.Theme = this.Theme;
            this.Model.File.ShowRecentFiles = this.ShowRecentFiles;

            this.themeManager.Theme = this.Theme;

            if (!this.Model.File.ShowRecentFiles)
            {
                this.Model.File.RecentFiles.Clear();
            }

            this.Model.Logging.LogPath = this.LogPath;
            this.Model.Logging.MinLogLevel = this.MinLogLevel;

            return base.OnSave()
                .DoAsync(_ => this.store.InsertObject(PreferencesKey, this.Model).Eager())
                .Eager();
        }

        protected override void CopyProperties()
        {
            this.Theme = this.Model.UI.Theme;
            this.ShowRecentFiles = this.Model.File.ShowRecentFiles;
            this.LogPath = this.Model.Logging.LogPath;
            this.MinLogLevel = this.Model.Logging.MinLogLevel;

            base.CopyProperties();
        }

        protected override bool InitialKindIsNewValue(Kind kind) =>
            !this.Model.Defaults.DefaultKinds.Contains(kind);

        protected override bool IsTagNew(TagModel tagModel) =>
            false;
    }
}
