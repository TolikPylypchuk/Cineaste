using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SeasonFormControl : UserControl
    {
        public SeasonFormControl()
            => this.InitializeComponent();

        public SeasonFormViewModel ViewModel { get; set; }
    }
}
