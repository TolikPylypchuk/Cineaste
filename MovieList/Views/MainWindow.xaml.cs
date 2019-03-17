using System.Windows;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.DataContext = viewModel;

            this.InitializeComponent();
            this.ViewModel.RestoreWindowState(this);
        }

        public MainViewModel ViewModel { get; }

        private void Window_Closing(object sender, System.EventArgs e)
            => this.ViewModel.SaveWindowState(this);
    }
}
