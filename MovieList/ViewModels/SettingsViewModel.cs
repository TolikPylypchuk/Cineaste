using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using HandyControl.Data;

using Microsoft.Extensions.Options;

using MovieList.Commands;
using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Events;
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

        private readonly IDbService dbService;
        private readonly IFileService fileService;
        private readonly MovieListViewModel movieListViewModel;
        private readonly IWritableOptions<Configuration> config;
        private readonly LoggingConfig loggingConfig;

        public SettingsViewModel(
            IDbService dbService,
            IFileService fileService,
            MovieListViewModel movieListViewModel,
            IWritableOptions<Configuration> config,
            IOptions<LoggingConfig> loggingConfig)
        {
            this.dbService = dbService;
            this.fileService = fileService;
            this.movieListViewModel = movieListViewModel;
            this.config = config;
            this.loggingConfig = loggingConfig.Value;

            this.notWatchedColor = config.Value.NotWatchedColor;
            this.notReleasedColor = config.Value.NotReleasedColor;
            this.defaultSeasonTitle = config.Value.DefaultSeasonTitle;
            this.defaultSeasonOriginalTitle = config.Value.DefaultSeasonOriginalTitle;
            this.databasePath = config.Value.DatabasePath;

            this.ChangeNotWatchedColor = new DelegateCommand<FrameworkElement>(element => Util.OpenColorPickerPopup(
                element, this.NotWatchedColor.ToString(), color => this.NotWatchedColor = color));

            this.ChangeNotReleasedColor = new DelegateCommand<FrameworkElement>(element => Util.OpenColorPickerPopup(
                element, this.NotReleasedColor.ToString(), color => this.NotReleasedColor = color));

            this.AddKind = new DelegateCommand(this.OnAddKind);
            this.RemoveKind = new DelegateCommand<KindViewModel>(async kind => await this.RemoveKindAsync(kind));

            this.Save = new DelegateCommand(async () => await this.SaveChangesAsync(), () => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(async () => await this.CancelChangesAsync(), () => this.CanCancelChanges);

            this.ViewLog = new DelegateCommand(this.OnViewLog);
        }

        public bool IsLoaded { get; private set; }

        public ICommand ChangeNotWatchedColor { get; }
        public ICommand ChangeNotReleasedColor { get; }

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

        [DefaultSeasonTitle(
            ErrorMessageResourceName = nameof(Messages.DefaultSeasonTitleInvalid),
            ErrorMessageResourceType = typeof(Messages))]
        public string DefaultSeasonTitle
        {
            get => this.defaultSeasonTitle;
            set
            {
                this.defaultSeasonTitle = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultSeasonTitle(
            ErrorMessageResourceName = nameof(Messages.DefaultSeasonTitleInvalid),
            ErrorMessageResourceType = typeof(Messages))]
        public string DefaultSeasonOriginalTitle
        {
            get => this.defaultSeasonOriginalTitle;
            set
            {
                this.defaultSeasonOriginalTitle = value;
                this.OnPropertyChanged();
            }
        }

        [File(
            ErrorMessageResourceName = nameof(Messages.InvalidPath),
            ErrorMessageResourceType = typeof(Messages))]
        [Required(
            ErrorMessageResourceName = nameof(Messages.DatabasePathRequired),
            ErrorMessageResourceType = typeof(Messages))]
        public string DatabasePath
        {
            get => this.databasePath;
            set
            {
                this.databasePath = value;
                this.OnPropertyChanged();
            }
        }

        public Func<string, OperationResult<bool>> VerifyDefaultSeasonTitle
            => this.Verify(nameof(DefaultSeasonTitle));

        public Func<string, OperationResult<bool>> VerifyDefaultSeasonOriginalTitle
            => this.Verify(nameof(DefaultSeasonOriginalTitle));

        public Func<string, OperationResult<bool>> VerifyDatabasePath
            => this.Verify(nameof(DatabasePath));

        public bool CanSaveChanges
            => this.IsLoaded && this.AreChangesPresent() && !this.HasErrors && this.Kinds.All(k => !k.HasErrors);

        public bool CanCancelChanges
            => this.IsLoaded && this.AreChangesPresent();

        public bool CanSaveOrCancelChanges
            => this.CanSaveChanges || this.CanCancelChanges;

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;

        public async Task LoadKindsAsync()
        {
            this.Kinds = await this.dbService.LoadAllKindsAsync();
            this.Kinds.CollectionChanged += (sender, e) => this.OnPropertyChanged(nameof(Kinds));

            foreach (var kind in Kinds)
            {
                kind.PropertyChanged += (sender, e) =>
                    this.OnPropertyChanged(nameof(Kinds));
            }

            this.IsLoaded = true;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Kinds) && this.IsLoaded)
            {
                this.KindsChanged = true;
            }

            if (propertyName != nameof(CanSaveChanges) &&
                propertyName != nameof(CanCancelChanges) &&
                propertyName != nameof(CanSaveOrCancelChanges))
            {
                this.OnCanSaveOrCancelChangesChanged();
            }
        }

        private void OnSettingsUpdated()
            => this.SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(
                new Configuration
                {
                    NotWatchedColor = this.NotWatchedColor,
                    NotReleasedColor = this.NotReleasedColor,
                    DefaultSeasonTitle = this.DefaultSeasonTitle,
                    DefaultSeasonOriginalTitle = this.DefaultSeasonOriginalTitle,
                    DatabasePath = this.DatabasePath
                },
                this.Kinds));

        private void OnAddKind()
        {
            var kind = new KindViewModel(new Kind
            {
                Name = Messages.NewKind,
                ColorForMovie = Colors.Black.ToString(),
                ColorForSeries = Colors.Black.ToString()
            });

            kind.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(Kinds));
            this.Kinds.Add(kind);
        }

        private async Task RemoveKindAsync(KindViewModel kind)
        {
            if (await this.dbService.CanDeleteKindAsync(kind))
            {
                this.Kinds.Remove(kind);
            } else
            {
                MessageBox.Show(
                        Messages.KindIsInUse,
                        Messages.Error,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
        }

        private async Task SaveChangesAsync()
        {
            if (this.KindsChanged)
            {
                await this.dbService.SaveKindsAsync(kinds);
                await this.movieListViewModel.LoadItemsAsync();

                this.KindsChanged = false;
            }

            if (this.IsConfigChanged())
            {
                this.config.Update(this.CopyConfig);
                this.CopyConfig(this.config.Value);
            }

            bool success = await this.fileService.TryMoveFileAsync(this.config.Value.DatabasePath, this.DatabasePath);

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

            this.OnCanSaveOrCancelChangesChanged();
            this.OnSettingsUpdated();
        }

        private async Task CancelChangesAsync()
        {
            this.NotWatchedColor = this.config.Value.NotWatchedColor;
            this.NotReleasedColor = this.config.Value.NotReleasedColor;

            this.DefaultSeasonTitle = this.config.Value.DefaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = this.config.Value.DefaultSeasonOriginalTitle;

            this.DatabasePath = this.config.Value.DatabasePath;

            await this.LoadKindsAsync();
            this.KindsChanged = false;

            this.OnCanSaveOrCancelChangesChanged();
        }

        private void OnCanSaveOrCancelChangesChanged()
        {
            CommandManager.InvalidateRequerySuggested();
            this.OnPropertyChanged(nameof(this.CanSaveChanges));
            this.OnPropertyChanged(nameof(this.CanCancelChanges));
            this.OnPropertyChanged(nameof(this.CanSaveOrCancelChanges));
        }

        private void CopyConfig(Configuration config)
        {
            config.NotWatchedColor = this.NotWatchedColor;
            config.NotReleasedColor = this.NotReleasedColor;
            config.DefaultSeasonTitle = this.DefaultSeasonTitle;
            config.DefaultSeasonOriginalTitle = this.DefaultSeasonOriginalTitle;
        }

        private bool AreChangesPresent()
            => this.IsConfigChanged() || this.KindsChanged;

        private bool IsConfigChanged()
            => this.NotWatchedColor != this.config.Value.NotWatchedColor ||
                this.NotReleasedColor != this.config.Value.NotReleasedColor ||
                this.DefaultSeasonTitle != this.config.Value.DefaultSeasonTitle ||
                this.DefaultSeasonOriginalTitle != this.config.Value.DefaultSeasonOriginalTitle ||
                this.DatabasePath != this.config.Value.DatabasePath;

        private void OnViewLog()
            => Process.Start("notepad.exe", this.loggingConfig.File.Path);
    }
}
