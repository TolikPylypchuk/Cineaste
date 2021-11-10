namespace Cineaste.Views.Filters;

public partial class CompositeFilterItemControl : ReactiveUserControl<CompositeFilterItemViewModel>
{
    private const string Green = "#00D778";
    private const string Blue = "#0078D7";

    public CompositeFilterItemControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Items, v => v.Filters.Items)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Composition)
                .Select(composition => composition == FilterComposition.And ? Green : Blue)
                .BindTo(this, v => v.ColorStripRectangle.Fill)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.AddItem, v => v.AddFilterButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.SwitchComposition, v => v.SwitchCompositionButton)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Composition)
                .Select(composition => composition == FilterComposition.And
                    ? Messages.SetFilterCompositionToOr
                    : Messages.SetFilterCompositionToAnd)
                .Subscribe(text => ToolTip.SetTip(this.SwitchCompositionButton, text))
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Simplify, v => v.SimplifyButton)
                .DisposeWith(disposables);

            this.ViewModel!.Simplify.CanExecute
                .BindTo(this, v => v.SimplifyButton.IsVisible)
                .DisposeWith(disposables);
        });
    }
}
