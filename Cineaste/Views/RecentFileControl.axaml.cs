namespace Cineaste.Views;

public partial class RecentFileControl : ReactiveUserControl<RecentFileViewModel>
{
    public RecentFileControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.ListNameTextBlock.Text = this.ViewModel!.File.Name;
            this.ListPathTextBlock.Text = this.ViewModel!.File.Path;

            this.Bind(this.ViewModel, vm => vm.IsSelected, v => v.IsSelectedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.GetObservable(DoubleTappedEvent)
                .Select(e => this.ViewModel.File.Path)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(this.ViewModel.HomePage.OpenRecentFile)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.IsSelected)
                .Subscribe(isSelected => this.Border.Background = isSelected
                    ? new SolidColorBrush(
                        (Color)(this.FindResource("SystemAccentColorLight3") ?? Colors.Transparent))
                    : Brushes.Transparent)
                .DisposeWith(disposables);
        });
    }
}
