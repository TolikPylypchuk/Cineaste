namespace Cineaste.Endpoints;

public static class ListEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapListEndpoints()
        {
            var list = endpoints.MapGroup("/list")
                .RequireAuthorization()
                .WithTags("List");

            list.MapGet("/", GetList)
                .ProducesProblem(() => new ListNotFoundException(Id.Create<CineasteList>()))
                .WithName(nameof(GetList))
                .WithSummary("Get the list");

            list.MapGet("/items", GetListItems)
                .ProducesProblem(() => new ListNotFoundException(Id.Create<CineasteList>()))
                .WithName(nameof(GetListItems))
                .WithSummary("Get list items");

            list.MapGet("/items/standalone", GetStandaloneListItems)
                .ProducesProblem(() => new ListNotFoundException(Id.Create<CineasteList>()))
                .WithName(nameof(GetStandaloneListItems))
                .WithSummary("Get standalone list items");

            list.MapGet("/items/{id}", GetListItem)
                .ProducesProblem(() => new ListItemNotFoundException(Guid.CreateVersion7()))
                .ProducesProblem(() => new ListNotFoundException(Id.Create<CineasteList>()))
                .WithName(nameof(GetListItem))
                .WithSummary("Get list item");

            list.MapGet(
                "items/parent-franchise-{parentFranchiseId}/{sequenceNumber}", GetListItemByParentFranchise)
                .ProducesProblem(() => new FranchiseNotFoundException(Id.Create<Franchise>()))
                .ProducesProblem(() => new FranchiseItemWithNumberNotFoundException(Id.Create<Franchise>(), 1))
                .ProducesProblem(() => new FranchiseItemNotFoundException(
                    Guid.CreateVersion7(), FranchiseItemType.Movie))
                .ProducesProblem(() => new ListNotFoundException(Id.Create<CineasteList>()))
                .WithName(nameof(GetListItemByParentFranchise))
                .WithSummary("Get list item by parent franchise and sequence number");

            return list;
        }
    }

    public static async Task<Ok<ListModel>> GetList(
        ListService listService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await listService.GetList(principal.ListId, token));

    public static async Task<Ok<OffsettableData<ListItemModel>>> GetListItems(
        int offset,
        int size,
        ListService listService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await listService.GetListItems(principal.ListId, offset, size, token));

    public static async Task<Ok<List<ListItemModel>>> GetStandaloneListItems(
        ListService listService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await listService.GetStandaloneListItems(principal.ListId, token));

    public static async Task<Ok<ListItemModel>> GetListItem(
        Guid id,
        ListService listService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await listService.GetListItem(principal.ListId, id, token));

    public static async Task<Ok<ListItemModel>> GetListItemByParentFranchise(
        Guid parentFranchiseId,
        int sequenceNumber,
        ListService listService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await listService.GetListItemByParentFranchise(
            principal.ListId, Id.For<Franchise>(parentFranchiseId), sequenceNumber, token));
}
