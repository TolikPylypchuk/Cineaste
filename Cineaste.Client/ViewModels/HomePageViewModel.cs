namespace Cineaste.Client.ViewModels;

public sealed class HomePageViewModel : ReactiveObject
{
    private readonly IPageNavigator pageNavigator;

    private readonly SourceCache<SimpleListModel, Guid> listsSource = new(list => list.Id);
    private readonly ReadOnlyObservableCollection<SimpleListModel> lists;

    public RemoteCall<List<SimpleListModel>> GetListsCall { get; }

    public ReadOnlyObservableCollection<SimpleListModel> Lists =>
        this.lists;

    public HomePageViewModel(IRemoteCallFactory remoteCallFactory, IPageNavigator pageNavigator)
    {
        this.pageNavigator = pageNavigator;

        this.GetListsCall = remoteCallFactory.Create((IListApi api) => api.GetLists());

        this.listsSource.Connect()
            .SortBy(list => list.Name)
            .Bind(out this.lists)
            .Subscribe();
    }

    public async Task LoadLists()
    {
        this.listsSource.Clear();

        await this.GetListsCall.Execute();

        if (this.GetListsCall.Result is not null)
        {
            this.listsSource.AddOrUpdate(this.GetListsCall.Result);
        }
    }

    public void NavigateToCreateList() =>
        this.pageNavigator.GoToCreateListPage();

    public void NavigateToList(SimpleListModel list) =>
        this.pageNavigator.GoToListPage(list.Handle);
}
