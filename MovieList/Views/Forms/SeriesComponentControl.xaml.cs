using System.Reactive.Disposables;
using System.Windows.Controls;

using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class SeriesComponentControlBase : ReactiveUserControl<SeriesComponentViewModel> { }

    public partial class SeriesComponentControl : SeriesComponentControlBase
    {
        public SeriesComponentControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Years, v => v.YearsTextBlock.Text)
                    .DisposeWith(disposables);

                this.Events().MouseLeftButtonUp
                    .Discard()
                    .InvokeCommand(this.ViewModel.Select)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.MoveUp, v => v.MoveUpMenuItem)
                    .DisposeWith(disposables);

                this.ViewModel.MoveUp.CanExecute
                    .BindTo(this, v => v.MoveUpMenuItem.Visibility)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.MoveDown, v => v.MoveDownMenuItem)
                    .DisposeWith(disposables);

                this.ViewModel.MoveDown.CanExecute
                    .BindTo(this, v => v.MoveDownMenuItem.Visibility)
                    .DisposeWith(disposables);
            });
        }
    }
}
