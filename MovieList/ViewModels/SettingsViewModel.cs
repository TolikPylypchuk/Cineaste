using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Options;
using MovieList.Services;
using MovieList.Validation;

namespace MovieList.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Color notWatchedColor;
        private Color notReleasedColor;
        private ObservableCollection<KindViewModel> kinds = new ObservableCollection<KindViewModel>();
        private bool kindsChanged;

        private string defaultSeasonTitle;
        private string defaultSeasonOriginalTitle;

        private string databasePath;

        private readonly App app;
        private readonly IWritableOptions<Configuration> config;
        private readonly LoggingConfig loggingConfig;

        public SettingsViewModel(App app, IWritableOptions<Configuration> config, IOptions<LoggingConfig> loggingConfig)
        {
            this.app = app;
            this.config = config;
            this.loggingConfig = loggingConfig.Value;

            this.notWatchedColor = config.Value.NotWatchedColor;
            this.notReleasedColor = config.Value.NotReleasedColor;
            this.defaultSeasonTitle = config.Value.DefaultSeasonTitle;
            this.defaultSeasonOriginalTitle = config.Value.DefaultSeasonOriginalTitle;
            this.databasePath = config.Value.DatabasePath;
        }

        public Color NotWatchedColor
        {
            get => this.notWatchedColor;
            set
            {
                this.notWatchedColor = value;
                this.OnPropertyChanged();
            }
        }

        public Color NotReleasedColor
        {
            get => this.notReleasedColor;
            set
            {
                this.notReleasedColor = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<KindViewModel> Kinds
        {
            get => this.kinds;
            set
            {
                this.kinds = value;
                this.OnPropertyChanged();
            }
        }

        public bool KindsChanged
        {
            get => this.kindsChanged;
            set
            {
                this.kindsChanged = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultSeasonTitle]
        public string DefaultSeasonTitle
        {
            get => this.defaultSeasonTitle;
            set
            {
                this.defaultSeasonTitle = value;
                this.Validate();
                this.OnPropertyChanged();
            }
        }

        [DefaultSeasonTitle]
        public string DefaultSeasonOriginalTitle
        {
            get => this.defaultSeasonOriginalTitle;
            set
            {
                this.defaultSeasonOriginalTitle = value;
                this.Validate();
                this.OnPropertyChanged();
            }
        }

        [File]
        public string DatabasePath
        {
            get => this.databasePath;
            set
            {
                this.databasePath = value;
                this.Validate();
                this.OnPropertyChanged();
            }
        }

        public async void LoadKindsAsync()
        {
            using var scope = this.app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IKindService>();

            this.Kinds = await service.LoadAllKindsAsync();
        }

        public bool CanSaveChanges()
            => this.AreChangesPresent() && !this.HasErrors && this.Kinds.All(k => !k.HasErrors);

        public bool CanCancelChanges()
            => this.AreChangesPresent();

        public async Task<bool> SaveChangesAsync()
        {
            if (this.KindsChanged)
            {
                using var scope = this.app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IKindService>();

                await service.SaveKindsAsync(kinds);
                this.KindsChanged = false;
            }

            if (this.IsConfigChanged())
            {
                this.config.Update(this.CopyConfig);
                this.CopyConfig(this.config.Value);
            }

            var fileService = this.app.ServiceProvider.GetRequiredService<IFileService>();

            bool success = await fileService.TryMoveFileAsync(this.config.Value.DatabasePath, this.DatabasePath);

            if (!success)
            {
                return false;
            }

            this.config.Update(config => config.DatabasePath = this.DatabasePath);
            this.config.Value.DatabasePath = this.DatabasePath;

            return true;
        }

        public async Task CancelChangesAsync()
        {
            this.NotWatchedColor = this.config.Value.NotWatchedColor;
            this.NotReleasedColor = this.config.Value.NotReleasedColor;

            this.DefaultSeasonTitle = this.config.Value.DefaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = this.config.Value.DefaultSeasonOriginalTitle;

            using var scope = this.app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IKindService>();

            this.Kinds = await service.LoadAllKindsAsync();
            this.KindsChanged = false;
        }

        public void ViewLog()
            => Process.Start("explorer.exe", $"/select, {this.loggingConfig.File.Path}");

        private void CopyConfig(Configuration config)
        {
            config.NotWatchedColor = this.NotWatchedColor;
            config.NotReleasedColor = this.NotReleasedColor;
            config.DefaultSeasonTitle = this.DefaultSeasonTitle;
            config.DefaultSeasonOriginalTitle = this.DefaultSeasonOriginalTitle;
        }

        private bool IsConfigChanged()
            => this.NotWatchedColor != this.config.Value.NotWatchedColor ||
                this.NotReleasedColor != this.config.Value.NotReleasedColor ||
                this.DefaultSeasonTitle != this.config.Value.DefaultSeasonTitle ||
                this.DefaultSeasonOriginalTitle != this.config.Value.DefaultSeasonOriginalTitle ||
                this.DatabasePath != this.config.Value.DatabasePath;

        private bool AreChangesPresent()
            =>  this.IsConfigChanged() || this.KindsChanged;
    }
}
