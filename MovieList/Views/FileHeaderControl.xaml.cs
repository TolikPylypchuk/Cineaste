using System.Reactive.Disposables;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class FileHaderControlBase : ReactiveUserControl<FileHeaderViewModel> { }

    public partial class FileHeaderControl : FileHaderControlBase
    {
        public FileHeaderControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.ListName, v => v.NameTextBlock.Text)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                    .DisposeWith(disposables);
            });
        }
    }
}
