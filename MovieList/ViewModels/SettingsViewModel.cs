using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Options;
using MovieList.Properties;
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
        private readonly MovieListViewModel movieListViewModel;
        private readonly IWritableOptions<Configuration> config;
        private readonly LoggingConfig loggingConfig;

        public SettingsViewModel(
            App app,
            MovieListViewModel movieListViewModel,
            IWritableOptions<Configuration> config,
            IOptions<LoggingConfig> loggingConfig)
        {
            this.app = app;
            this.movieListViewModel = movieListViewModel;
            this.config = config;
            this.loggingConfig = loggingConfig.Value;

            this.notWatchedColor = config.Value.NotWatchedColor;
            this.notReleasedColor = config.Value.NotReleasedColor;
            this.defaultSeasonTitle = config.Value.DefaultSeasonTitle;
            this.defaultSeasonOriginalTitle = config.Value.DefaultSeasonOriginalTitle;
            this.databasePath = config.Value.DatabasePath;

            this.AddKind = new DelegateCommand(_ => this.OnAddKind());
            this.RemoveKind = new DelegateCommand(this.OnRemoveKind);
            this.Save = new DelegateCommand(async _ => await this.SaveChangesAsync(), _ => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(async _ => await this.CancelChangesAsync(), _ => this.CanCancelChanges);
            this.ViewLog = new DelegateCommand(_ => this.OnViewLog());
        }

        public bool IsLoaded { get; private set; }

        public ICommand AddKind { get; }
        public ICommand RemoveKind { get; }
        public ICommand Save { get; }
        public ICommand Cancel { get; }
        public ICommand ViewLog { get; }

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
                this.OnPropertyChanged();
            }
        }

        public bool CanSaveChanges
            => this.IsLoaded && this.AreChangesPresent() && !this.HasErrors && this.Kinds.All(k => !k.HasErrors);

        public bool CanCancelChanges
            => this.IsLoaded && this.AreChangesPresent();

        public async void LoadKindsAsync()
        {
            using var scope = this.app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IKindService>();

            this.Kinds = await service.LoadAllKindsAsync();
            this.IsLoaded = true;
        }

        public void OnKindsChanged()
        {
            if (this.IsLoaded)
            {
                this.KindsChanged = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void OnAddKind()
        {
            this.Kinds.Add(new KindViewModel(new Kind
            {
                Name = Messages.NewKind,
                ColorForMovie = Colors.Black.ToString(),
                ColorForSeries = Colors.Black.ToString()
            }));

            this.KindsChanged = true;
            CommandManager.InvalidateRequerySuggested();
        }

        private async void OnRemoveKind(object obj)
        {
            if (obj is KindViewModel kind)
            {
                using var scope = this.app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IKindService>();

                if (await service.CanRemoveKindAsync(kind))
                {
                    this.Kinds.Remove(kind);
                    this.KindsChanged = true;
                    CommandManager.InvalidateRequerySuggested();
                } else
                {
                    MessageBox.Show(
                           Messages.KindIsInUse,
                           Messages.Error,
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
                }
            }
        }

        private async Task SaveChangesAsync()
        {
            if (this.KindsChanged)
            {
                using var scope = this.app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IKindService>();

                await service.SaveKindsAsync(kinds);
                await this.movieListViewModel.LoadItemsAsync();

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
                MessageBox.Show(
                       Messages.SavingSettingsFailed,
                       Messages.Error,
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);

                return;
            }

            this.config.Update(config => config.DatabasePath = this.DatabasePath);
            this.config.Value.DatabasePath = this.DatabasePath;

            CommandManager.InvalidateRequerySuggested();
        }

        private async Task CancelChangesAsync()
        {
            this.NotWatchedColor = this.config.Value.NotWatchedColor;
            this.NotReleasedColor = this.config.Value.NotReleasedColor;

            this.DefaultSeasonTitle = this.config.Value.DefaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = this.config.Value.DefaultSeasonOriginalTitle;

            this.DatabasePath = this.config.Value.DatabasePath;

            using var scope = this.app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IKindService>();

            this.Kinds = await service.LoadAllKindsAsync();
            this.KindsChanged = false;

            CommandManager.InvalidateRequerySuggested();
        }

        public void OnViewLog()
            => Process.Start("notepad.exe", this.loggingConfig.File.Path);

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
