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
    }
}
