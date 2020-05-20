using System.Reactive.Disposables;

using MovieList.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class KindFormControlBase : ReactiveUserControl<KindFormViewModel> { }

    public partial class KindFormControl : KindFormControlBase
    {
        public KindFormControl()
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
