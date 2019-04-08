using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using Microsoft.Extensions.DependencyInjection;

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

        private readonly App app;
        private readonly IWritableOptions<Configuration> config;

        public SettingsViewModel(App app, IWritableOptions<Configuration> config)
        {
            this.app = app;
            this.config = config;
            this.notWatchedColor = config.Value.NotWatchedColor;
            this.notReleasedColor = config.Value.NotReleasedColor;
            this.defaultSeasonTitle = config.Value.DefaultSeasonTitle;
            this.defaultSeasonOriginalTitle = config.Value.DefaultSeasonOriginalTitle;
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

        public async Task SaveChangesAsync()
        {
            if (this.IsConfigChanged())
            {
                this.config.Update(this.CopyConfig);
                this.CopyConfig(this.config.Value);
            }

            using var scope = this.app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IKindService>();

            await service.SaveKindsAsync(kinds);
            this.KindsChanged = false;
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
                this.DefaultSeasonOriginalTitle != this.config.Value.DefaultSeasonOriginalTitle;

        private bool AreChangesPresent()
            =>  this.IsConfigChanged() || this.KindsChanged;
    }
}
