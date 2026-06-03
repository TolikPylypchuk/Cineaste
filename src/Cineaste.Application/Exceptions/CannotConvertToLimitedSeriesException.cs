namespace Cineaste.Application.Exceptions;

public sealed class CannotConvertToLimitedSeriesException(Id<Series> seriesId, Exception? innerException = null)
    : Exception($"Cannot convert series {seriesId} to a limited series", innerException)
{
    public Id<Series> SeriesId { get; } = seriesId;
}
