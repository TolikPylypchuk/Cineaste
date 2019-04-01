using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;

using Microsoft.Extensions.DependencyInjection;

using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Options;
using MovieList.Services;

namespace MovieList.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Color notWatchedColor;
        private Color notReleasedColor;
        private ObservableCollection<Kind> kinds;

        private readonly App app;
        private readonly IWritableOptions<Configuration> config;

        public SettingsViewModel(App app, IWritableOptions<Configuration> config)
        {
            this.app = app;
            this.config = config;
            this.notWatchedColor = config.Value.NotWatchedColor;
            this.notReleasedColor = config.Value.NotReleasedColor;
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

        public ObservableCollection<Kind> Kinds
        {
            get => this.kinds;
            set
            {
                this.kinds = value;
                this.OnPropertyChanged();
            }
        }

        public async void LoadKindsAsync()
        {
            using var scope = app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IKindService>();

            this.Kinds = await service.LoadAllKindsAsync();
        }

        public bool CanSaveChanges()
            => this.NotWatchedColor != this.config.Value.NotWatchedColor ||
                this.NotReleasedColor != this.config.Value.NotReleasedColor;

        public async Task SaveChangesAsync()
        {
            if (this.NotWatchedColor != this.config.Value.NotWatchedColor ||
                this.NotReleasedColor != this.config.Value.NotReleasedColor)
            {
                this.config.Update(config =>
                {
                    config.NotWatchedColor = this.NotWatchedColor;
                    config.NotReleasedColor = this.NotReleasedColor;
                });

                this.config.Value.NotWatchedColor = this.NotWatchedColor;
                this.config.Value.NotReleasedColor = this.NotReleasedColor;
            }

            using var scope = app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IKindService>();

            await service.SaveKindsAsync(kinds);
        }

        public Task CancelChangesAsync()
        {
            this.NotWatchedColor = this.config.Value.NotWatchedColor;
            this.NotReleasedColor = this.config.Value.NotReleasedColor;

            return Task.CompletedTask;
        }
    }
}
