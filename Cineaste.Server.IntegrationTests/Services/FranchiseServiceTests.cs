namespace Cineaste.Server.Services;

using Cineaste.Core;
using Cineaste.Core.Domain;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Franchise;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class FranchiseServiceTests : ServiceTestsBase
{
    private readonly Movie movie;
    private readonly Series series;
    private readonly ILogger<FranchiseService> logger;

    public FranchiseServiceTests()
    {
        this.movie = this.CreateMovie(this.List);
        this.series = this.CreateSeries(this.List);

        this.logger = new NullLogger<FranchiseService>();
    }

    [Fact(DisplayName = "GetFranchise should return the correct franchise")]
    public async Task GetFranchiseShouldReturnCorrectFranchise()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(this.movie);
        franchise.AddSeries(this.series);

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync();

        var model = await franchiseService.GetFranchise(franchise.Id);

        AssertTitles(franchise, model);

        Assert.Equal(franchise.Id.Value, model.Id);
        Assert.Equal(franchise.ShowTitles, model.ShowTitles);
        Assert.Equal(franchise.IsLooselyConnected, model.IsLooselyConnected);
        Assert.Equal(franchise.ContinueNumbering, model.ContinueNumbering);

        Assert.Equal(franchise.Children.Count, model.Items.Count);

        foreach (var (item, itemModel) in franchise.Children.OrderBy(item => item.SequenceNumber)
            .Zip(model.Items.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(item.Title.Name, itemModel.Title);

            Assert.Equal(
                item.Select(movie => movie.Year, series => series.StartYear, franchise => franchise.StartYear ?? 0),
                itemModel.StartYear);

            Assert.Equal(
                item.Select(movie => movie.Year, series => series.EndYear, franchise => franchise.EndYear ?? 0),
                itemModel.EndYear);

            Assert.Equal(
                item.Select(
                    movie => FranchiseItemType.Movie,
                    series => FranchiseItemType.Series,
                    franchise => FranchiseItemType.Franchise),
                itemModel.Type);
        }
    }

    [Fact(DisplayName = "GetFranchise should throw if franchise isn't found")]
    public async Task GetFranchiseShouldThrowIfNotFound()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var dummyId = Id.CreateNew<Franchise>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => franchiseService.GetFranchise(dummyId));

        Assert.Equal("NotFound.Franchise", exception.MessageCode);
        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }
}
