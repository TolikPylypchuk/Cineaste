using System.Windows.Controls;
using System.Windows.Input;

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

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = this.ViewModel?.CanSaveChanges() ?? false;

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveChanges();
            CommandManager.InvalidateRequerySuggested();
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = this.ViewModel?.CanSaveChanges() ?? false;

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.CancelChanges();
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
