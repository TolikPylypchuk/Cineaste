using System.Windows;
using MovieList.Config;
using MovieList.Options;

namespace MovieList.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWritableOptions<UIConfig> configOptions;

        public MainViewModel(IWritableOptions<UIConfig> configOptions)
            => this.configOptions = configOptions;

        public UIConfig Config => this.configOptions.Value;

        public void RestoreWindowState(Window window)
        {
            window.Width = this.Config.Width;
            window.Height = this.Config.Height;
            window.Left = this.Config.Left;
            window.Top = this.Config.Top;
            window.WindowState = this.Config.IsMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        public void SaveWindowState(Window window)
            => this.configOptions.Update(config =>
            {
                config.Width = window.Width;
                config.Height = window.Height;
                config.Top = window.Top;
                config.Left = window.Left;
                config.IsMaximized = window.WindowState == WindowState.Maximized;
            });
    }
}
