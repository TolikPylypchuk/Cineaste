using System.Reactive.Disposables;

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
            });
        }
    }
}
