using System.Windows;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            this.DataContext = this.ViewModel = viewModel;
            this.ViewModel.MainWindow = this;

            this.InitializeComponent();

            this.ViewModel.InitializeViewModels();
            this.ViewModel.RestoreWindowState();
        }

        public MainViewModel ViewModel { get; }

        private void Window_Closing(object sender, System.EventArgs e)
            => this.ViewModel.SaveWindowState();
    }
}
