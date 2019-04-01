using System.Windows.Media;

using MovieList.Config;
using MovieList.Options;

namespace MovieList.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Color notWatchedColor;
        private Color notReleasedColor;
        private readonly IWritableOptions<Configuration> config;

        public SettingsViewModel(IWritableOptions<Configuration> config)
        {
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

        public bool CanSaveChanges()
            => this.NotWatchedColor != this.config.Value.NotWatchedColor ||
                this.NotReleasedColor != this.config.Value.NotReleasedColor;

        public void SaveChanges()
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
        }

        public void CancelChanges()
        {
            this.NotWatchedColor = this.config.Value.NotWatchedColor;
            this.NotReleasedColor = this.config.Value.NotReleasedColor;
        }
    }
}
