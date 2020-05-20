using System.Reactive.Disposables;

using MovieList.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class SettingsFormControlBase : ReactiveUserControl<SettingsFormViewModel> { }

    public partial class SettingsFormControl : SettingsFormControlBase
    {
        public SettingsFormControl()
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
