namespace Cineaste.Views;

public partial class FileControl : ReactiveUserControl<FileViewModel>
{
    public FileControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Content, v => v.Host.ViewModel)
                .DisposeWith(disposables);

            Observable.FromEventPattern<NavigationViewItemInvokedEventArgs>(
                h => this.Navigation.ItemInvoked += h, h => this.Navigation.ItemInvoked -= h)
                .Select(e => e.EventArgs.InvokedItemContainer)
                .DistinctUntilChanged()
                .Select(item => item == this.ListItem
                    ? this.ViewModel!.SwitchToList
                    : this.ViewModel!.SwitchToSettings)
                .SubscribeAsync(command => command.Execute())
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Content)
                .WhereNotNull()
                .DistinctUntilChanged()
                .Select(content => content switch
                {
                    FileMainContentViewModel => this.ListItem,
                    _ => this.Navigation.SettingsItem
                })
                .BindTo(this, v => v.Navigation.SelectedItem)
                .DisposeWith(disposables);

            this.ViewModel!.IsInitialized
                .Where(isInitialized => isInitialized)
                .Select(_ => this.ListItem)
                .Take(1)
                .BindTo(this, v => v.Navigation.SelectedItem)
                .DisposeWith(disposables);
        });
    }
}
