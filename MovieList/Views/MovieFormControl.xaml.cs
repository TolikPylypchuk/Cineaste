using System.Windows;
using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MovieFormControl : UserControl
    {
        private MovieFormViewModel viewModel;

        public MovieFormControl()
            => this.InitializeComponent();

        public MovieFormViewModel ViewModel
        {
            get => this.viewModel;
            set
            {
                this.viewModel = value;
                this.viewModel.PropertyChanged += (sender, e) =>
                {
                    this.IsWatchedCheckBox.GetBindingExpression(IsEnabledProperty).UpdateTarget();
                    this.IsNotReleasedCheckBox.GetBindingExpression(IsEnabledProperty).UpdateTarget();
                };
            }
        }

        private void IsWatchedCheckBox_Checked(object sender, RoutedEventArgs e)
            => this.IsNotReleasedCheckBox.IsChecked = false;

        private void IsWatchedCheckBox_Unchecked(object sender, RoutedEventArgs e)
            => this.IsNotReleasedCheckBox.IsChecked = true;

        private void IsNotReleasedCheckBox_Checked(object sender, RoutedEventArgs e)
            => this.IsWatchedCheckBox.IsChecked = false;

        private void IsNotReleasedCheckBox_Unchecked(object sender, RoutedEventArgs e)
            => this.IsWatchedCheckBox.IsChecked = true;
    }
}
