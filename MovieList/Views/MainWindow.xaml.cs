using System.Windows;

using MovieList.Config;
using MovieList.Options;

namespace MovieList.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(IWritableOptions<UIConfig> config)
        {
            this.Config = config;
            this.WindowState = config.Value.IsMaximized ? WindowState.Maximized : WindowState.Normal;
            this.Height = config.Value.Height;
            this.Width = config.Value.Width;
            this.Top = config.Value.Top;
            this.Left = config.Value.Left;

            this.InitializeComponent();
        }

        public IWritableOptions<UIConfig> Config { get; }

        private void Window_Closing(object sender, System.EventArgs e)
            => this.Config.Update(config =>
            {
                config.Height = this.Height;
                config.Width = this.Width;
                config.Top = this.Top;
                config.Left = this.Left;
                config.IsMaximized = this.WindowState == WindowState.Maximized;
            });
    }
}
