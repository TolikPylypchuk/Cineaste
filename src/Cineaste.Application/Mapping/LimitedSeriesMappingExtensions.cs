namespace Cineaste.Application.Mapping;

public static class LimitedSeriesMappingExtensions
{
    extension(LimitedSeries limitedSeries)
    {
        public LimitedSeriesModel ToLimitedSeriesModel(IPosterUrlProvider posterUrlProvider) =>
            new(
                limitedSeries.Id.Value,
                limitedSeries.AllTitles.ToTitleModels(isOriginal: false),
                limitedSeries.AllTitles.ToTitleModels(isOriginal: true),
                limitedSeries.Period.ToPeriodModel(),
                limitedSeries.WatchStatus,
                limitedSeries.ReleaseStatus,
                limitedSeries.Channel,
                limitedSeries.Kind.ToListKindModel(),
                limitedSeries.ImdbId?.Value,
                limitedSeries.RottenTomatoesId?.Value,
                limitedSeries.RottenTomatoesSubId?.Value,
                limitedSeries.ActiveColor.HexValue,
                limitedSeries.ListItem?.SequenceNumber ?? 0,
                posterUrlProvider.GetPosterUrl(limitedSeries.Id, limitedSeries.PosterHash),
                limitedSeries.FranchiseItem.ToFranchiseItemInfoModel());

        public void Update(Validated<LimitedSeriesRequest> request, SeriesKind kind)
        {
            limitedSeries.ReplaceTitles(
                request.Value.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: false);

            limitedSeries.ReplaceTitles(
                request.Value.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: true);

            limitedSeries.Period = request.Value.Period.ToReleasePeriod();
            limitedSeries.WatchStatus = request.Value.WatchStatus;
            limitedSeries.ReleaseStatus = request.Value.ReleaseStatus;
            limitedSeries.Channel = request.Value.Channel;
            limitedSeries.Kind = kind;
            limitedSeries.ImdbId = ImdbId.Nullable(request.Value.ImdbId);
            limitedSeries.RottenTomatoesId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesId);
            limitedSeries.RottenTomatoesSubId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesSubId);

            limitedSeries.ListItem?.SetProperties(limitedSeries);
        }
    }

    extension(Validated<LimitedSeriesRequest> request)
    {
        public LimitedSeries ToLimitedSeries(Id<LimitedSeries> id, SeriesKind kind) =>
            new(
                id,
                request.Value.ToTitles(),
                request.Value.Period.ToReleasePeriod(),
                request.Value.WatchStatus,
                request.Value.ReleaseStatus,
                request.Value.Channel,
                kind)
            {
                ImdbId = ImdbId.Nullable(request.Value.ImdbId),
                RottenTomatoesId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesId),
                RottenTomatoesSubId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesSubId)
            };
    }
}
