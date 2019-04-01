using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MovieListControl : UserControl
    {
        public MovieListControl()
        {
            this.InitializeComponent();
        }

        public MovieListViewModel ViewModel { get; set; }

        private void MovieListControl_Loaded(object sender, RoutedEventArgs e)
            => Task.Factory.StartNew(this.ViewModel.LoadItemsAsync);
    }
}
