using System.Reactive.Disposables;

using MovieList.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class TagFormControlBase : ReactiveUserControl<TagFormViewModel> { }

    public partial class TagFormControl : TagFormControlBase
    {
        public TagFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
