using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class TitleFormControlBase : ReactiveUserControl<TitleFormViewModel> { }

    public partial class TitleFormControl : TitleFormControlBase
    {
        public TitleFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                    .DisposeWith(disposables);

                this.ViewModel.Delete.CanExecute
                    .Select(canDelete => canDelete ? Visibility.Visible : Visibility.Hidden)
                    .BindTo(this, v => v.DeleteButton.Visibility)
                    .DisposeWith(disposables);
            });
        }
    }
}
