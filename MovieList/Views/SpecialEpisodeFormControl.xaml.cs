using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SpecialEpisodeFormControl : UserControl
    {
        public SpecialEpisodeFormControl()
            => this.InitializeComponent();

        public SpecialEpisodeFormViewModel ViewModel { get; set; }
    }
}
