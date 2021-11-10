namespace Cineaste.Views.Forms.Preferences;

public partial class TagItemControl : ReactiveUserControl<TagItemViewModel>
{
    public TagItemControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Name, v => v.TagChip.Text)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Color, v => v.TagChip.TagBrush)
                .DisposeWith(disposables);

            Observable.CombineLatest(
                this.WhenAnyValue(v => v.ViewModel!.Category),
                this.WhenAnyValue(v => v.ViewModel!.Description),
                (category, description) => !String.IsNullOrEmpty(description)
                    ? $"{category} | {description}"
                    : category)
                .Subscribe(toolTip => ToolTip.SetTip(this, toolTip))
                .DisposeWith(disposables);

            if (this.ViewModel!.CanSelect)
            {
                this.TagChip.IsClickable = true;

                this.TagChip.GetObservable(Chip.ClickEvent)
                    .Discard()
                    .InvokeCommand(this.ViewModel!.Select)
                    .DisposeWith(disposables);
            }

            this.TagChip.GetObservable(Chip.DeletedEvent)
                .Discard()
                .InvokeCommand(this.ViewModel.Delete)
                .DisposeWith(disposables);
        });
    }
}
