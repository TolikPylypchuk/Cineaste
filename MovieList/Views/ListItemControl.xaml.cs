using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

using MovieList.Core.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ListItemControlBase : ReactiveUserControl<ListItemViewModel> { }

    public partial class ListItemControl : ListItemControlBase
    {
        private static readonly SolidColorBrush HighlightBrush = new(new Color { A = 255, R = 237, G = 231, B = 246 });

        public ListItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.DisplayNumber, v => v.NumberTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.Title, v => v.TitleTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.OriginalTitle, v => v.OriginalTitleTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.Year, v => v.YearTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Item.IsHighlighted)
                    .Select(isHighlighted => isHighlighted ? HighlightBrush : Brushes.Transparent)
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.ItemBorder.Background)
                    ?.DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Item.Color,
                    v => v.Foreground,
                    color => new SolidColorBrush
                    {
                        Color = (Color?)ColorConverter.ConvertFromString(color) ?? Colors.Black
                    })
                    ?.DisposeWith(disposables);
            });
        }
    }
}
