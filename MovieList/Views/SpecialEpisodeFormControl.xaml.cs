using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SpecialEpisodeFormControl : UserControl
    {
        private SpecialEpisodeFormViewModel viewModel;

        public SpecialEpisodeFormControl()
            => this.InitializeComponent();

        public SpecialEpisodeFormViewModel ViewModel
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
