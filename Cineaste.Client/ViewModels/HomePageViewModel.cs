namespace Cineaste.Client.ViewModels;

public sealed class HomePageViewModel : ReactiveObject
{
    private readonly IListService listService;
    private readonly IPageNavigator pageNavigator;

    private readonly SourceCache<SimpleListModel, Guid> listsSource = new(list => list.Id);
    private readonly ReadOnlyObservableCollection<SimpleListModel> lists;

    [Reactive]
    public bool IsLoading { get; set; }

    public ReadOnlyObservableCollection<SimpleListModel> Lists =>
        this.lists;

    public HomePageViewModel(IListService listService, IPageNavigator pageNavigator)
    {
        this.listService = listService;
        this.pageNavigator = pageNavigator;

        this.listsSource.Connect()
            .SortBy(list => list.Name)
            .Bind(out this.lists)
            .Subscribe();
    }

    public async Task Initialize()
    {
        this.IsLoading = true;
        this.listsSource.Clear();

        var lists = await this.listService.GetLists();

        this.listsSource.AddOrUpdate(lists);

        this.IsLoading = false;
    }

    public void NavigateToCreateList() =>
        this.pageNavigator.GoToCreateListPage();

    public void NavigateToList(SimpleListModel list) =>
        this.pageNavigator.GoToListPage(list.Handle);
}
