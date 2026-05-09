namespace Cineaste.Application.Exceptions;

public abstract class NotFoundException(
    string? message = null,
    Exception? innerException = null)
    : Exception(message, innerException);

public sealed class LimitedSeriesNotFoundException(
    Id<LimitedSeries> limitedSeriesId,
    Exception? innerException = null)
    : NotFoundException($"Limited series with ID {limitedSeriesId} not found", innerException)
{
    public Id<LimitedSeries> LimitedSeriesId { get; } = limitedSeriesId;
}

public sealed class LimitedSeriesPosterNotFoundException(
    Id<LimitedSeries> limitedSeriesId,
    Exception? innerException = null)
    : NotFoundException($"Poster for limited series with ID {limitedSeriesId} not found", innerException)
{
    public Id<LimitedSeries> LimitedSeriesId { get; } = limitedSeriesId;
}

public sealed class ListNotFoundException(
    Id<CineasteList> listId,
    Exception? innerException = null)
    : NotFoundException($"List with ID {listId} not found", innerException)
{
    public Id<CineasteList> ListId { get; } = listId;
}

public sealed class ListItemNotFoundException(
    Guid itemId,
    Exception? innerException = null)
    : NotFoundException($"List item with ID {itemId} not found", innerException)
{
    public Guid ItemId { get; } = itemId;
}

public sealed class MovieNotFoundException(
    Id<Movie> movieId,
    Exception? innerException = null)
    : NotFoundException($"Movie with ID {movieId} not found", innerException)
{
    public Id<Movie> MovieId { get; } = movieId;
}

public sealed class MovieKindNotFoundException(
    Id<MovieKind> kindId,
    Exception? innerException = null)
    : NotFoundException($"Movie kind with ID {kindId} not found", innerException)
{
    public Id<MovieKind> KindId { get; } = kindId;
}

public sealed class MoviePosterNotFoundException(
    Id<Movie> movieId,
    Exception? innerException = null)
    : NotFoundException($"Poster for movie with ID {movieId} not found", innerException)
{
    public Id<Movie> MovieId { get; } = movieId;
}

public sealed class SeriesNotFoundException(
    Id<Series> seriesId,
    Exception? innerException = null)
    : NotFoundException($"Series with ID {seriesId} not found", innerException)
{
    public Id<Series> SeriesId { get; } = seriesId;
}

public sealed class SeriesKindNotFoundException(
    Id<SeriesKind> kindId,
    Exception? innerException = null)
    : NotFoundException($"Series kind with ID {kindId} not found", innerException)
{
    public Id<SeriesKind> KindId { get; } = kindId;
}

public sealed class SeriesPosterNotFoundException(
    Id<Series> seriesId,
    Exception? innerException = null)
    : NotFoundException($"Poster for series with ID {seriesId} not found", innerException)
{
    public Id<Series> SeriesId { get; } = seriesId;
}

public sealed class SeasonPartNotFoundException(
    Id<SeasonPart> partId,
    Exception? innerException = null)
    : NotFoundException($"Season part with ID {partId} not found", innerException)
{
    public Id<SeasonPart> PartId { get; } = partId;
}

public sealed class SeasonPosterNotFoundException(
    Id<SeasonPart> partId,
    Exception? innerException = null)
    : NotFoundException($"Poster for season part with ID {partId} not found", innerException)
{
    public Id<SeasonPart> PartId { get; } = partId;
}

public sealed class SpecialEpisodeNotFoundException(
    Id<SpecialEpisode> episodeId,
    Exception? innerException = null)
    : NotFoundException($"Special episode with ID {episodeId} not found", innerException)
{
    public Id<SpecialEpisode> EpisodeId { get; } = episodeId;
}

public sealed class SpecialEpisodePosterNotFoundException(
    Id<SpecialEpisode> episodeId,
    Exception? innerException = null)
    : NotFoundException($"Poster for special episode with ID {episodeId} not found", innerException)
{
    public Id<SpecialEpisode> EpisodeId { get; } = episodeId;
}

public sealed class FranchiseNotFoundException(
    Id<Franchise> franchiseId,
    Exception? innerException = null)
    : NotFoundException($"Franchise with ID {franchiseId} not found", innerException)
{
    public Id<Franchise> FranchiseId { get; } = franchiseId;
}

public sealed class FranchiseItemNotFoundException(
    Guid id,
    FranchiseItemType itemType,
    Exception? innerException = null)
    : NotFoundException($"List item with ID {id} not found", innerException)
{
    public FranchiseItemNotFoundException(FranchiseItem franchiseItem, Exception? innerException = null)
        : this(GetItemId(franchiseItem), GetItemType(franchiseItem), innerException)
    { }

    public Guid ItemId { get; } = id;

    public FranchiseItemType ItemType { get; } = itemType;

    private static Guid GetItemId(FranchiseItem item) => item.Select(
            movie => movie.Id.Value,
            series => series.Id.Value,
            limitedSeries => limitedSeries.Id.Value,
            franchise => franchise.Id.Value);

    private static FranchiseItemType GetItemType(FranchiseItem item) => item.Select(
        movie => FranchiseItemType.Movie,
        series => FranchiseItemType.Series,
        series => FranchiseItemType.LimitedSeries,
        franchise => FranchiseItemType.Franchise);
}

public sealed class FranchiseItemWithNumberNotFoundException(
    Id<Franchise> franchiseId,
    int sequenceNumber,
    Exception? innerException = null)
    : NotFoundException($"Item #{sequenceNumber} of franchise with ID {franchiseId} not found", innerException)
{
    public Id<Franchise> FranchiseId { get; } = franchiseId;
    public int SequenceNumber { get; } = sequenceNumber;
}

public sealed class FranchiseItemsNotFoundException(
    IEnumerable<Id<Movie>> movieIds,
    IEnumerable<Id<Series>> seriesIds,
    IEnumerable<Id<Franchise>> franchiseIds,
    Exception? innerException = null)
    : NotFoundException($"Franchise items not found", innerException)
{
    public IReadOnlyCollection<Id<Movie>> MovieIds { get; } = movieIds.ToList().AsReadOnly();
    public IReadOnlyCollection<Id<Series>> SeriesIds { get; } = seriesIds.ToList().AsReadOnly();
    public IReadOnlyCollection<Id<Franchise>> FranchiseIds { get; } = franchiseIds.ToList().AsReadOnly();
}

public sealed class FranchisePosterNotFoundException(
    Id<Franchise> franchiseId,
    Exception? innerException = null)
    : NotFoundException($"Poster for franchise with ID {franchiseId} not found", innerException)
{
    public Id<Franchise> FranchiseId { get; } = franchiseId;
}
