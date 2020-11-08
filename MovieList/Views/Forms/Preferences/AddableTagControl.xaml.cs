using System.Reactive.Disposables;

using MovieList.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class AddableTagControlBase : ReactiveUserControl<AddableTagViewModel> { }

    public partial class AddableTagControl : AddableTagControlBase
    {
        public AddableTagControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Name, v => v.NameTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Category,
                    v => v.CategoryTextBlock.Text,
                    category => $"({category})")
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Color, v => v.ColorCircle.Fill)
                    .DisposeWith(disposables);
            });
        }
    }
}
