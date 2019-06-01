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
            => Task.Factory.StartNew(this.ViewModel.LoadItemsAsync);

        private void MovieListControl_PreviewKeyDown(object sender, KeyEventArgs e)
            => e.Handled = e.Key == Key.Up || e.Key == Key.Down;

        private void MovieListControl_PreviewKeyUp(object sender, KeyEventArgs e)
            => e.Handled = this.ViewModel.MoveSelectedItem(e.Key);

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
