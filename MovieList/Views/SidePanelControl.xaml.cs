using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SidePanelControl : UserControl
    {
        public SidePanelControl()
        {
            this.InitializeComponent();
        }

        public SidePanelViewModel ViewModel { get; set; }
    }
}
