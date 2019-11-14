using System.Reactive.Disposables;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class SettingsControlBase : ReactiveUserControl<SettingsViewModel> { }

    public partial class SettingsControl : SettingsControlBase
    {
        public SettingsControl()
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
