namespace Cineaste.Server.Data;

using System.ComponentModel;
using System.Globalization;

internal sealed class TestDataProvider
{
    private readonly CineasteDbContext dbContext;

    public TestDataProvider(CineasteDbContext dbContext) =>
        this.dbContext = dbContext;

    public async Task CreateTestDataIfNeeded()
    {
        int listsCount = await this.dbContext.Lists.CountAsync();

        if (listsCount != 0)
        {
            return;
        }

        this.dbContext.Lists.AddRange(
            this.CreateList("Test List 1"),
            this.CreateList("Test List 2"),
            this.CreateList("Test List 3"),
            this.CreateList("Test List 4"),
            this.CreateList("Test List 5"));

        await this.dbContext.SaveChangesAsync();
    }

    private CineasteList CreateList(string name)
    {
        var config = new ListConfiguration(
            Id.Create<ListConfiguration>(),
            CultureInfo.InvariantCulture,
            "Season #",
            "Season #",
            new(ListSortOrder.ByTitle, ListSortDirection.Ascending, ListSortOrder.ByYear, ListSortDirection.Ascending));

        return new CineasteList(Id.Create<CineasteList>(), name, config);
    }
}
