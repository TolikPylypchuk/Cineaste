namespace Cineaste.Client.Services.Http;

using System.Net;

public sealed class ListService : IListService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions;

    public ListService(HttpClient client, IOptions<JsonSerializerOptions> jsonOptions)
    {
        this.client = client;
        this.jsonOptions = jsonOptions.Value;
    }

    public async Task<List<SimpleListModel>> GetLists()
    {
        try
        {
            return await this.client.GetFromJsonAsync<List<SimpleListModel>>("/api/lists", this.jsonOptions) ?? new();
        } catch (HttpRequestException)
        {
            return new();
        }
    }

    public async Task<ListModel?> GetList(string handle)
    {
        try
        {
            return await this.client.GetFromJsonAsync<ListModel?>($"/api/lists/{handle}", this.jsonOptions);
        } catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<List<ListCultureModel>> GetAllCultures()
    {
        try
        {
            return await this.client.GetFromJsonAsync<List<ListCultureModel>?>($"/api/cultures", this.jsonOptions)
                ?? new();
        } catch (HttpRequestException)
        {
            return new();
        }
    }

    public async Task<SimpleListModel?> CreateList(CreateListRequest request)
    {
        try
        {
            var message = await this.client.PostAsJsonAsync($"/api/lists", request, this.jsonOptions);

            return message.StatusCode == HttpStatusCode.OK
                ? await JsonSerializer.DeserializeAsync<SimpleListModel>(
                    message.Content.ReadAsStream(), this.jsonOptions)
                : null;
        } catch (HttpRequestException)
        {
            return null;
        }
    }
}
