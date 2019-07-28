using System.Windows.Controls;
using System.Windows.Input;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MovieSeriesFormControl : UserControl
    {
        public MovieSeriesFormControl()
            => this.InitializeComponent();

        public MovieSeriesFormViewModel ViewModel { get; set; }

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
