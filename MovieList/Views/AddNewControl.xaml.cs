using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class AddNewControl : UserControl
    {
        public AddNewControl()
            => this.InitializeComponent();

        public AddNewViewModel ViewModel { get; set; }
    }
}
