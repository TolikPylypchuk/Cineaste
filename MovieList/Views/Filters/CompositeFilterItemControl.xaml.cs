using System.Reactive.Disposables;
using System.Reactive.Linq;

using MovieList.Core.ViewModels.Filters;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class CompositeFilterItemControlBase : ReactiveUserControl<CompositeFilterItemViewModel> { }

    public partial class CompositeFilterItemControl : CompositeFilterItemControlBase
    {
        private const string Green = "#43A047";
        private const string Blue = "#1E88E5";

        public CompositeFilterItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Items, v => v.Filters.ItemsSource)
                    ?.DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Composition)
                    .Select(composition => composition == FilterComposition.And ? Green : Blue)
                    .BindTo(this, v => v.ColorStripRectangle.Fill)
                    ?.DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.SwitchComposition, v => v.SwitchCompositionButton)
                    ?.DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Composition)
                    .Select(composition => composition == FilterComposition.And
                        ? Messages.SetFilterCompositionToOr
                        : Messages.SetFilterCompositionToAnd)
                    .BindTo(this, v => v.SwitchCompositionButton.ToolTip)
                    ?.DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.AddItem, v => v.AddFilterButton)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
