using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MovieListControl : UserControl
    {
        public MovieListControl()
            => this.InitializeComponent();

        public MovieListViewModel ViewModel { get; set; }

        private void MovieListControl_Loaded(object sender, RoutedEventArgs e)
            => Task.Factory.StartNew(this.ViewModel.LoadItemsAsync)
                .ContinueWith(_ =>
                    this.Dispatcher.Invoke(() =>
                    {
                        this.List.Visibility = Visibility.Visible;
                        this.LoadingProgressBar.Visibility = Visibility.Collapsed;
                    }));

        private void MoreButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                button.ContextMenu.IsOpen = true;
            }

            e.Handled = true;
        }
    }
}
