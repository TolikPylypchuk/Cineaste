using System.Reactive.Disposables;

using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class MovieFormControlBase : ReactiveUserControl<MovieFormViewModel> { }

    public partial class MovieFormControl : MovieFormControlBase
    {
        public MovieFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);
            });
        }
    }
}
