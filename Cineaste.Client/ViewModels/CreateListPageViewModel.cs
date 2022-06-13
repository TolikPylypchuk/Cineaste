namespace Cineaste.Client.ViewModels;

public sealed class CreateListPageViewModel : ReactiveValidationObject
{
    private static readonly SimpleCultureModel DefaultCultureModel = new(
        CultureInfo.InvariantCulture.ToString(), CultureInfo.InvariantCulture.DisplayName);

    private const string DefaultDefaultSeasonTitle = "Season #";

    private readonly IPageNavigator pageNavigator;

    private readonly SourceCache<SimpleCultureModel, string> allCulturesSource = new(culture => culture.Id);
    private readonly ReadOnlyObservableCollection<SimpleCultureModel> allCultures;

    [Reactive]
    public string Name { get; set; } = String.Empty;

    public string Handle { [ObservableAsProperty] get; } = String.Empty;

    [Reactive]
    public SimpleCultureModel Culture { get; set; } = DefaultCultureModel;

    [Reactive]
    public string DefaultSeasonTitle { get; set; } = DefaultDefaultSeasonTitle;

    [Reactive]
    public string DefaultSeasonOriginalTitle { get; set; } = DefaultDefaultSeasonTitle;

    public ReadOnlyObservableCollection<SimpleCultureModel> AllCultures =>
        this.allCultures;

    public RemoteCall<List<SimpleCultureModel>> GetAllCulturesCall { get; }

    public RemoteCall<SimpleListModel> CreateListCall { get; }

    public ValidationHelper NameRule { get; }
    public ValidationHelper DefaultSeasonTitleRule { get; }
    public ValidationHelper DefaultSeasonOriginalTitleRule { get; }

    public CreateListPageViewModel(IRemoteCallFactory remoteCallFactory, IPageNavigator pageNavigator)
    {
        this.pageNavigator = pageNavigator;

        this.GetAllCulturesCall = remoteCallFactory.Create((ICultureApi api) => api.GetAllCultures());

        this.CreateListCall = remoteCallFactory.Create((IListApi api) => api.CreateList(new CreateListRequest(
            this.Name, this.Culture.Id, this.DefaultSeasonTitle, this.DefaultSeasonOriginalTitle)));

        this.WhenAnyValue(vm => vm.Name)
            .Select(ListUtils.CreateHandleFromName)
            .ToPropertyEx(this, vm => vm.Handle);

        this.NameRule = this.ValidationRule(vm => vm.Name, this.UsingValidator<CreateListRequest>());

        this.DefaultSeasonTitleRule = this.ValidationRule(
            vm => vm.DefaultSeasonTitle, this.UsingValidator<CreateListRequest>());

        this.DefaultSeasonOriginalTitleRule = this.ValidationRule(
            vm => vm.DefaultSeasonOriginalTitle, this.UsingValidator<CreateListRequest>());

        this.allCulturesSource.Connect()
            .Bind(out this.allCultures)
            .Subscribe();
    }

    public async Task Initialize()
    {
        this.Name = String.Empty;
        this.Culture = DefaultCultureModel;

        await this.LoadCultures();
    }

    public async Task LoadCultures()
    {
        await this.GetAllCulturesCall.Execute();

        if (this.GetAllCulturesCall.Result is not null)
        {
            this.allCulturesSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(this.GetAllCulturesCall.Result);
            });
        }
    }

    public void GoToHomePage() =>
        this.pageNavigator.GoToHomePage();

    public async Task CreateList()
    {
        await this.CreateListCall.Execute();

        if (this.CreateListCall.Result is not null)
        {
            pageNavigator.GoToListPage(this.CreateListCall.Result.Handle);
        }
    }
}
