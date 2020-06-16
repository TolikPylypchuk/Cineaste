using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using Akavache;

using MovieList.Preferences;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Constants;

namespace MovieList.ViewModels.Forms.Preferences
{
    public sealed class PreferencesFormViewModel : SettingsFormBase<UserPreferences, PreferencesFormViewModel>
    {
        private readonly IBlobCache store;

        public PreferencesFormViewModel(
            UserPreferences userPreferences,
            IBlobCache? store = null,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(userPreferences, resourceManager, scheduler)
        {
            this.store = store ?? Locator.Current.GetService<IBlobCache>(StoreKey);

            this.CopyProperties();
            this.CanNeverDelete();
            this.EnableChangeTracking();
        }

        [Reactive]
        public bool ShowRecentFiles { get; set; }

        [Reactive]
        public string LogPath { get; set; } = String.Empty;

        [Reactive]
        public int MinLogLevel { get; set; }

        protected override PreferencesFormViewModel Self
            => this;

        protected override IObservable<UserPreferences> OnSave()
        {
            this.Model.File.ShowRecentFiles = this.ShowRecentFiles;

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
            this.ShowRecentFiles = this.Model.File.ShowRecentFiles;
            this.LogPath = this.Model.Logging.LogPath;
            this.MinLogLevel = this.Model.Logging.MinLogLevel;

            base.CopyProperties();
        }
    }
}
