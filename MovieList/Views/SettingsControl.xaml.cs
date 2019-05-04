using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
            => this.ViewModel.OnKindsChanged();

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
            => this.ViewModel.OnKindsChanged();
    }
}
