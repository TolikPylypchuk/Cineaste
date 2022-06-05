namespace Cineaste.Client.ViewModels;

public sealed class HomePageViewModel : ReactiveObject
{
    private readonly IApiExecutorFactory api;
    private readonly IPageNavigator pageNavigator;

    private readonly SourceCache<SimpleListModel, Guid> listsSource = new(list => list.Id);
    private readonly ReadOnlyObservableCollection<SimpleListModel> lists;

    [Reactive]
    public bool IsLoading { get; set; }

    [Reactive]
    public bool LoadingFailed { get; set; }

    public ReadOnlyObservableCollection<SimpleListModel> Lists =>
        this.lists;

    public HomePageViewModel(IApiExecutorFactory api, IPageNavigator pageNavigator)
    {
        this.api = api;
        this.pageNavigator = pageNavigator;

        this.listsSource.Connect()
            .SortBy(list => list.Name)
            .Bind(out this.lists)
            .Subscribe();
    }

    public async Task LoadLists()
    {
        this.IsLoading = true;
        this.LoadingFailed = false;

        this.listsSource.Clear();

        var response = await this.api.For<IListApi>().Fetch(api => api.GetLists());

        if (response is ApiSuccess<List<SimpleListModel>> lists)
        {
            this.listsSource.AddOrUpdate(lists.Content);
        } else
        {
            this.LoadingFailed = true;
        }

        this.IsLoading = false;
    }

    public void NavigateToCreateList() =>
        this.pageNavigator.GoToCreateListPage();

    public void NavigateToList(SimpleListModel list) =>
        this.pageNavigator.GoToListPage(list.Handle);
}