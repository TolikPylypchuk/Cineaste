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
    }
}
