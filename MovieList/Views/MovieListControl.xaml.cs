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

        private void MovieListControl_PreviewKeyDown(object sender, KeyEventArgs e)
            => e.Handled = e.Key == Key.Up || e.Key == Key.Down;

        private void MovieListControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (this.List.SelectedItem != null)
            {
                if (e.Key == Key.Up)
                {
                    this.SelectAndScroll(
                        (this.List.SelectedIndex + this.ViewModel.Items.Count - 1) % this.ViewModel.Items.Count);
                    e.Handled = true;
                } else if (e.Key == Key.Down)
                {
                    this.SelectAndScroll(
                        (this.List.SelectedIndex + this.ViewModel.Items.Count + 1) % this.ViewModel.Items.Count);
                    e.Handled = true;
                } else if (e.Key == Key.Home)
                {
                    this.SelectAndScroll(0);
                    e.Handled = true;
                } else if (e.Key == Key.End)
                {
                    this.SelectAndScroll(this.ViewModel.Items.Count - 1);
                    e.Handled = true;
                }
            } else if (e.Key == Key.Up || e.Key == Key.Home)
            {
                this.SelectAndScroll(this.ViewModel.Items.Count - 1);
                e.Handled = true;
            } else if (e.Key == Key.Down || e.Key == Key.End)
            {
                this.SelectAndScroll(0);
                e.Handled = true;
            }
        }

        private void MoreButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                button.ContextMenu.IsOpen = true;
            }

            e.Handled = true;
        }

        private void SelectAndScroll(int index)
        {
            this.List.SelectedIndex = index;

            this.List.UpdateLayout();
            this.List.ScrollIntoView(this.List.SelectedItem);
        }
    }
}
