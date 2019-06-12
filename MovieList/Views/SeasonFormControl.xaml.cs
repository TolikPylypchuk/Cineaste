using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SeasonFormControl : UserControl
    {
        private SeasonFormViewModel viewModel;

        public SeasonFormControl()
            => this.InitializeComponent();

        public SeasonFormViewModel ViewModel
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
