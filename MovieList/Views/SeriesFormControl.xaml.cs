using System.Windows.Controls;
using System.Windows.Input;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SeriesFormControl : UserControl
    {
        private SeriesFormViewModel viewModel;

        public SeriesFormControl()
            => this.InitializeComponent();

        public SeriesFormViewModel ViewModel
        {
            get => this.viewModel;
            set
            {
                this.viewModel = value;
                this.viewModel.PropertyChanged += (sender, e) =>
                {
                    this.IsWatchedCheckBox.GetBindingExpression(IsEnabledProperty).UpdateTarget();
                    this.IsMiniseriesCheckBox.GetBindingExpression(IsEnabledProperty).UpdateTarget();
                };
            }
        }

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
