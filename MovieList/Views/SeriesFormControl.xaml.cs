using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SeriesFormControl : UserControl
    {
        public SeriesFormControl()
        {
            this.InitializeComponent();
        }

        public SeriesFormViewModel ViewModel { get; set; }
    }
}
