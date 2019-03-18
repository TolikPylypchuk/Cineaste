using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            this.InitializeComponent();
        }

        public SettingsViewModel ViewModel { get; set; }
    }
}
