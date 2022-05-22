namespace Cineaste.Client.Services;

public sealed class ListService : IListService
{
    private readonly HttpClient client;

    public ListService(HttpClient client) =>
        this.client = client;

    public async Task<List<SimpleListModel>> GetLists() =>
        (await this.client.GetFromJsonAsync<List<SimpleListModel>>("/api/lists")) ?? new();
}
