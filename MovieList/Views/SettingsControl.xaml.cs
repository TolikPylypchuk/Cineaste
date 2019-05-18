using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
            => this.InitializeComponent();

        public SettingsViewModel ViewModel { get; set; }

        private void SettingsControl_Loaded(object sender, RoutedEventArgs e)
            => Task.Factory.StartNew(this.ViewModel.LoadKindsAsync);
    }
}
