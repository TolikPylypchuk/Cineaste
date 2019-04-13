using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MovieSeriesFormControl : UserControl
    {
        public MovieSeriesFormControl()
            => this.InitializeComponent();

        public MovieSeriesFormViewModel ViewModel { get; set; }
    }
}
