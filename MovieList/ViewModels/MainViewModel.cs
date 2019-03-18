using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using MovieList.Config;
using MovieList.Options;
using MovieList.Views;

namespace MovieList.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly App app;
        private readonly IWritableOptions<UIConfig> configOptions;

        public MainViewModel(App app, IWritableOptions<UIConfig> configOptions)
        {
            this.app = app;
            this.configOptions = configOptions;
        }

        public void SetComponentViewModels(MainWindow window)
        {
            window.ListControl.ViewModel = this.app.ServiceProvider.GetRequiredService<MovieListViewModel>();
            window.SettingsControl.ViewModel = this.app.ServiceProvider.GetRequiredService<SettingsViewModel>();
        }

        public void RestoreWindowState(Window window)
        {
            window.Width = this.configOptions.Value.Width;
            window.Height = this.configOptions.Value.Height;
            window.Left = this.configOptions.Value.Left;
            window.Top = this.configOptions.Value.Top;
            window.WindowState = this.configOptions.Value.IsMaximized ? WindowState.Maximized : WindowState.Normal;
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
