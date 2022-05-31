namespace Cineaste.Client.ViewModels;

using System.Globalization;

using Nito.Comparers;

public sealed class ListPageViewModel : ReactiveObject
{
    private readonly IListService listService;

    private readonly SourceCache<ListItemModel, Guid> itemsSource = new(item => item.Id);
    private readonly ReadOnlyObservableCollection<ListItemModel> items;

    private readonly BehaviorSubject<IComparer<ListItemModel>> comparer = new(null!);

    private Dictionary<Guid, ListItemModel> itemsById = new();

    [Reactive]
    public string Handle { get; set; } = String.Empty;

    [Reactive]
    public ListModel? List { get; set; }

    [Reactive]
    public bool IsLoading { get; set; }

    public ReadOnlyObservableCollection<ListItemModel> Items =>
        this.items;

    public ListPageViewModel(IListService listService)
    {
        this.listService = listService;

        this.itemsSource.Connect()
            .Sort(this.comparer)
            .Bind(out this.items)
            .Subscribe();
    }

    public async Task Initialize()
    {
        this.IsLoading = true;

        this.List = await this.listService.GetList(this.Handle);

        if (this.List is not null)
        {
            this.itemsById = this.List.Movies
                .Concat(this.List.Series)
                .Concat(this.List.Franchises)
                .ToDictionary(item => item.Id, item => item);

            this.SetComparer();

            this.itemsSource.Edit(list =>
            {
                list.AddOrUpdate(this.List.Movies);
                list.AddOrUpdate(this.List.Series);
                list.AddOrUpdate(this.List.Franchises);
            });
        }

        this.IsLoading = false;
    }

    private void SetComparer()
    {
        if (this.List is null)
        {
            return;
        }

        var culture = CultureInfo.GetCultureInfo(this.List.Config.Culture);

        var comparerByYear = ComparerBuilder.For<ListItemModel>()
            .OrderBy(item => item.StartYear, descending: false)
            .ThenBy(item => item.EndYear, descending: false);

        this.comparer.OnNext(new ListItemTitleComparer(
            culture,
            comparerByYear,
            id => this.itemsById[id],
            item => item.Title));
    }
}
