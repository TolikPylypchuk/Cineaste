namespace Cineaste.Client.ViewModels;

using System.Globalization;

using Nito.Comparers;

public sealed class ListPageViewModel : ReactiveObject
{
    private readonly IListApi listApi;

    private readonly SourceCache<ListItemModel, Guid> itemsSource = new(item => item.Id);
    private readonly ReadOnlyObservableCollection<ListItemModel> items;

    private readonly BehaviorSubject<IComparer<ListItemModel>> comparer = new(null!);

    private Dictionary<Guid, ListItemModel> itemsById = new();

    [Reactive]
    public string Handle { get; set; } = String.Empty;

    [Reactive]
    public ListModel? List { get; private set; }

    [Reactive]
    public bool IsLoading { get; private set; }

    [Reactive]
    public bool FailedLoading { get; private set; }

    public ReadOnlyObservableCollection<ListItemModel> Items =>
        this.items;

    public ListPageViewModel(IListApi listApi)
    {
        this.listApi = listApi;

        this.itemsSource.Connect()
            .Sort(this.comparer)
            .Bind(out this.items)
            .Subscribe();
    }

    public async Task Initialize(string handle)
    {
        this.Handle = handle;

        this.IsLoading = true;
        this.FailedLoading = false;

        var response = await this.listApi.GetList(this.Handle);

        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            this.List = response.Content;

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
        } else
        {
            this.List = null;

            this.itemsSource.Clear();
            this.itemsById.Clear();

            this.FailedLoading = true;
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
