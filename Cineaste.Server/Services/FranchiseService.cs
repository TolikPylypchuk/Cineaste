namespace Cineaste.Server.Services;

[AutoConstructor]
[GenerateAutoInterface]
public sealed partial class FranchiseService : IFranchiseService
{
    private readonly CineasteDbContext dbContext;
    private readonly ILogger<FranchiseService> logger;

    public async Task<FranchiseModel> GetFranchise(Id<Franchise> id)
    {
        this.logger.LogDebug("Getting the franchise with id: {Id}", id.Value);

        var franchise = await this.FindFranchise(id);
        return franchise.ToFranchiseModel();
    }

    private async Task<Franchise> FindFranchise(Id<Franchise> id)
    {
        var franchise = await this.dbContext.Franchises
            .Include(franchise => franchise.Titles)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Movie!.Titles)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series!.Titles)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series!.Seasons)
                    .ThenInclude(season => season!.Periods)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series!.SpecialEpisodes)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Franchise!.Titles)
            .AsSplitQuery()
            .SingleOrDefaultAsync(franchise => franchise.Id == id);

        return franchise is not null ? franchise : throw this.NotFound(id);
    }

    private Exception NotFound(Id<Franchise> id) =>
        new NotFoundException(Resources.Franchise, $"Could not find a franchise with id {id.Value}")
            .WithProperty(id);
}
