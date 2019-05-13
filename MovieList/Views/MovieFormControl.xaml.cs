using System;
using System.Windows.Controls;

using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class MovieFormControl : UserControl
    {
        public MovieFormControl()
            => this.InitializeComponent();

        public MovieFormViewModel ViewModel { get; set; }

        private void Year_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && Int32.TryParse(textBox.Text, out int year))
            {
                this.IsWatchedCheckBox.GetBindingExpression(IsEnabledProperty).UpdateTarget();
                this.IsNotReleasedCheckBox.GetBindingExpression(IsEnabledProperty).UpdateTarget();

                if (year > DateTime.Now.Year)
                {
                    this.ViewModel.Movie.IsReleased = false;
                } else if (year < DateTime.Now.Year)
                {
                    this.ViewModel.Movie.IsReleased = true;
                }
            }
        }
    }
}
