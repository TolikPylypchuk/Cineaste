using System.Windows;

using Microsoft.Extensions.Logging;

using MovieList.Config;
using MovieList.Config.Logging;
using MovieList.Options;

namespace MovieList.Views
{
    public partial class MainWindow : Window
    {
        private readonly IWritableOptions<UIConfig> configOptions;

        public MainWindow(IWritableOptions<UIConfig> config)
        {
            this.configOptions = config;
            this.WindowState = config.Value.IsMaximized ? WindowState.Maximized : WindowState.Normal;
            this.Height = config.Value.Height;
            this.Width = config.Value.Width;
            this.Top = config.Value.Top;
            this.Left = config.Value.Left;

            this.InitializeComponent();
        }

        private void Window_Closing(object sender, System.EventArgs e)
            => this.configOptions.Update(config =>
            {
                config.Height = this.Height;
                config.Width = this.Width;
                config.Top = this.Top;
                config.Left = this.Left;
                config.IsMaximized = this.WindowState == WindowState.Maximized;
            });
    }
}
