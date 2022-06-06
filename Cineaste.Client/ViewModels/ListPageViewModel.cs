namespace Cineaste.Client.ViewModels;

using System.Globalization;

using Nito.Comparers;

public sealed class ListPageViewModel : ReactiveObject
{
    private readonly SourceCache<ListItemModel, Guid> itemsSource = new(item => item.Id);
    private readonly ReadOnlyObservableCollection<ListItemModel> items;

    private readonly BehaviorSubject<IComparer<ListItemModel>> comparer = new(null!);

    private Dictionary<Guid, ListItemModel> itemsById = new();

    [Reactive]
    public string Handle { get; set; } = String.Empty;

    [Reactive]
    public ListModel? List { get; private set; }

    public ReadOnlyObservableCollection<ListItemModel> Items =>
        this.items;

    public RemoteCall<ListModel> GetListCall { get; }

    public ListPageViewModel(IRemoteCallFactory remoteCallFactory)
    {
        this.GetListCall = remoteCallFactory.Create((IListApi api) => api.GetList(this.Handle));

        this.itemsSource.Connect()
            .Sort(this.comparer)
            .Bind(out this.items)
            .Subscribe();
    }

    public async Task Initialize(string handle)
    {
        this.Handle = handle;

        await this.GetListCall.Execute();

        if (this.GetListCall.Result is not null)
        {
            this.List = this.GetListCall.Result;

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
        }
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
