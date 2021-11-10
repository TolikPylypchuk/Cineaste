namespace Cineaste.Views.Forms;

public partial class FranchiseEntryControl : ReactiveUserControl<FranchiseEntryViewModel>
{
    public FranchiseEntryControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.BindFields(disposables);
            this.BindCommands(disposables);
        });
    }

    private void BindFields(CompositeDisposable disposables)
    {
        this.OneWayBind(this.ViewModel, vm => vm.NumberToDisplay, v => v.DisplayNumberTextBlock.Text)
            .DisposeWith(disposables);

        this.OneWayBind(this.ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text)
            .DisposeWith(disposables);

        this.OneWayBind(this.ViewModel, vm => vm.Years, v => v.YearsTextBlock.Text)
            .DisposeWith(disposables);
    }

    private void BindCommands(CompositeDisposable disposables)
    {
        this.GetObservable(PointerReleasedEvent)
            .Discard()
            .InvokeCommand(this.ViewModel!.Select)
            .DisposeWith(disposables);

        this.BindCommand(this.ViewModel!, vm => vm.MoveUp, v => v.MoveUpMenuItem)
            .DisposeWith(disposables);

        this.ViewModel.MoveUp.CanExecute
            .BindTo(this, v => v.MoveUpMenuItem.IsVisible)
            .DisposeWith(disposables);

        this.BindCommand(this.ViewModel!, vm => vm.MoveDown, v => v.MoveDownMenuItem)
            .DisposeWith(disposables);

        this.ViewModel.MoveDown.CanExecute
            .BindTo(this, v => v.MoveDownMenuItem.IsVisible)
            .DisposeWith(disposables);

        this.BindCommand(this.ViewModel!, vm => vm.HideDisplayNumber, v => v.HideDisplayNumberMenuItem)
            .DisposeWith(disposables);

        this.ViewModel.HideDisplayNumber.CanExecute
            .BindTo(this, v => v.HideDisplayNumberMenuItem.IsVisible)
            .DisposeWith(disposables);

        this.BindCommand(this.ViewModel!, vm => vm.ShowDisplayNumber, v => v.ShowDisplayNumberMenuItem)
            .DisposeWith(disposables);

        this.ViewModel.ShowDisplayNumber.CanExecute
            .BindTo(this, v => v.ShowDisplayNumberMenuItem.IsVisible)
            .DisposeWith(disposables);

        this.BindCommand(this.ViewModel!, vm => vm.Delete, v => v.DetachMenuItem)
            .DisposeWith(disposables);

        this.ViewModel.Delete.CanExecute
            .BindTo(this, v => v.DetachMenuItem.IsVisible)
            .DisposeWith(disposables);
    }
}
