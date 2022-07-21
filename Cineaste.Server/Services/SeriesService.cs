namespace Cineaste.Server.Services;

[AutoConstructor]
[GenerateAutoInterface]
public sealed partial class SeriesService : ISeriesService
{
    private readonly CineasteDbContext dbContext;
    private readonly ILogger<SeriesService> logger;

    public async Task<SeriesModel> GetSeries(Id<Series> id)
    {
        this.logger.LogDebug("Getting the series with id: {Id}", id.Value);

        var series = await this.FindSeries(id);
        return series.ToSeriesModel();
    }

    private async Task<Series> FindSeries(Id<Series> id)
    {
        var series = await this.dbContext.Series
            .Include(series => series.Titles)
            .Include(series => series.Kind)
            .Include(series => series.Seasons)
                .ThenInclude(season => season.Titles)
            .Include(series => series.Seasons)
                .ThenInclude(season => season.Periods)
            .Include(series => series.SpecialEpisodes)
                .ThenInclude(episode => episode.Titles)
            .Include(series => series.Tags)
            .AsSplitQuery()
            .SingleOrDefaultAsync(series => series.Id == id);

        return series is not null ? series : throw this.NotFound(id);
    }

    private Exception NotFound(Id<Series> id) =>
        new NotFoundException(Resources.Series, $"Could not find a series with id {id.Value}")
            .WithProperty(id);
}
