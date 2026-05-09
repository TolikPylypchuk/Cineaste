namespace Cineaste.Application.Mapping;

public static class ReleasePeriodMappingExtensions
{
    extension(ReleasePeriod period)
    {
        public ReleasePeriodModel ToPeriodModel() =>
            new(
                period.StartMonth.Value,
                period.StartYear,
                period.EndMonth.Value,
                period.EndYear,
                period.EpisodeCount,
                period.IsSingleDayRelease);
    }

    extension(ReleasePeriodRequest request)
    {
        public ReleasePeriod ToReleasePeriod() =>
            new(
                request.StartMonth.ToMonth(),
                request.StartYear,
                request.EndMonth.ToMonth(),
                request.EndYear,
                request.IsSingleDayRelease,
                request.EpisodeCount);
    }
}
