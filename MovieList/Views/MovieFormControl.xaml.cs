using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MovieFormControl : UserControl
    {
        public MovieFormControl()
            => this.InitializeComponent();

        public MovieFormViewModel ViewModel { get; set; }
    }
}
