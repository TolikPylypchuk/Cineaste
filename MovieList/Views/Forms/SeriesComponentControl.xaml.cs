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

                this.Events().MouseDoubleClick
                    .Discard()
                    .InvokeCommand(this.ViewModel.Select)
                    .DisposeWith(disposables);
            });
        }
    }
}
