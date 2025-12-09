namespace Cineaste.Application.Mapping;

public static class SeriesMappingExtensions
{
    extension(Series series)
    {
        public SeriesModel ToSeriesModel(IPosterUrlProvider posterUrlProvider) =>
            new(
                series.Id.Value,
                series.AllTitles.ToTitleModels(isOriginal: false),
                series.AllTitles.ToTitleModels(isOriginal: true),
                series.WatchStatus,
                series.ReleaseStatus,
                series.Kind.ToListKindModel(),
                [.. series.Seasons.Select(season => season.ToSeasonModel(series, posterUrlProvider))],
                [.. series.SpecialEpisodes.Select(episode => episode.ToSpecialEpisodeModel(series, posterUrlProvider))],
                series.ImdbId?.Value,
                series.RottenTomatoesId?.Value,
                series.ActiveColor.HexValue,
                series.ListItem?.SequenceNumber ?? 0,
                posterUrlProvider.GetPosterUrl(series.Id, series.PosterHash),
                series.FranchiseItem.ToFranchiseItemInfoModel());

        public void Update(Validated<SeriesRequest> request, SeriesKind kind)
        {
            series.ReplaceTitles(
                request.Value.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: false);

            series.ReplaceTitles(
                request.Value.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: true);

            series.WatchStatus = request.Value.WatchStatus;
            series.ReleaseStatus = request.Value.ReleaseStatus;
            series.Kind = kind;
            series.ImdbId = ImdbId.Nullable(request.Value.ImdbId);
            series.RottenTomatoesId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesId);

            series.UpdateSeasons(request.Value.Seasons);
            series.UpdateSpecialEpisodes(request.Value.SpecialEpisodes);

            series.ListItem?.SetProperties(series);
        }

        private void UpdateSeasons(IReadOnlyCollection<SeasonRequest> requests)
        {
            var requestIds = requests.Select(req => req.Id).WhereValueNotNull().ToHashSet();

            foreach (var season in series.Seasons.Where(e => !requestIds.Contains(e.Id.Value)).ToList())
            {
                series.RemoveSeason(season);
            }

            foreach (var season in series.Seasons.Where(e => requestIds.Contains(e.Id.Value)))
            {
                season.Update(requests.First(req => req.Id == season.Id.Value));
            }

            foreach (var request in requests.Where(req => !req.Id.HasValue))
            {
                series.AddSeason(request.ToSeason());
            }
        }

        private void UpdateSpecialEpisodes(IReadOnlyCollection<SpecialEpisodeRequest> requests)
        {
            var requestIds = requests.Select(req => req.Id).WhereValueNotNull().ToHashSet();

            foreach (var episode in series.SpecialEpisodes.Where(e => !requestIds.Contains(e.Id.Value)).ToList())
            {
                series.RemoveSpecialEpisode(episode);
            }

            foreach (var episode in series.SpecialEpisodes.Where(e => requestIds.Contains(e.Id.Value)))
            {
                episode.Update(requests.First(req => req.Id == episode.Id.Value));
            }

            foreach (var request in requests.Where(req => !req.Id.HasValue))
            {
                series.AddSpecialEpisode(request.ToSpecialEpisode());
            }
        }
    }

    extension(Validated<SeriesRequest> request)
    {
        public Series ToSeries(Id<Series> id, SeriesKind kind) =>
            new(
                id,
                request.Value.ToTitles(),
                request.Value.Seasons.Select(ToSeason),
                request.Value.SpecialEpisodes.Select(ToSpecialEpisode),
                request.Value.WatchStatus,
                request.Value.ReleaseStatus,
                kind)
            {
                ImdbId = ImdbId.Nullable(request.Value.ImdbId),
                RottenTomatoesId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesId)
            };
    }

    extension(Season season)
    {
        private SeasonModel ToSeasonModel(Series series, IPosterUrlProvider posterUrlProvider) =>
            new(
                season.Id.Value,
                season.AllTitles.ToTitleModels(isOriginal: false),
                season.AllTitles.ToTitleModels(isOriginal: true),
                season.SequenceNumber,
                season.WatchStatus,
                season.ReleaseStatus,
                season.Channel,
                [.. season.Periods
                .Select(period => period.ToPeriodModel(series, posterUrlProvider))
                .OrderBy(period => period.StartYear)
                .ThenBy(period => period.StartMonth)
                .ThenBy(period => period.EndYear)
                .ThenBy(period => period.EndMonth)]);

        private void Update(SeasonRequest request)
        {
            season.ReplaceTitles(
                request.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: false);

            season.ReplaceTitles(
                request.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: true);

            season.WatchStatus = request.WatchStatus;
            season.ReleaseStatus = request.ReleaseStatus;
            season.Channel = request.Channel;
            season.SequenceNumber = request.SequenceNumber;

            var requestIds = request.Periods.Select(req => req.Id).WhereValueNotNull().ToHashSet();

            foreach (var period in season.Periods.Where(e => !requestIds.Contains(e.Id.Value)).ToList())
            {
                season.RemovePeriod(period);
            }

            foreach (var period in season.Periods.Where(e => requestIds.Contains(e.Id.Value)))
            {
                period.Update(request.Periods.First(req => req.Id == period.Id.Value));
            }

            foreach (var periodRequest in request.Periods.Where(req => !req.Id.HasValue))
            {
                season.AddPeriod(periodRequest.ToPeriod());
            }
        }
    }

    extension(Period period)
    {
        private PeriodModel ToPeriodModel(Series series, IPosterUrlProvider posterUrlProvider) =>
            new(
                period.Id.Value,
                period.StartMonth,
                period.StartYear,
                period.EndMonth,
                period.EndYear,
                period.EpisodeCount,
                period.IsSingleDayRelease,
                period.RottenTomatoesId?.Value,
                posterUrlProvider.GetPosterUrl(series.Id, period.Id, period.PosterHash));

        private void Update(PeriodRequest request)
        {
            period.StartMonth = request.StartMonth;
            period.StartYear = request.StartYear;
            period.EndMonth = request.EndMonth;
            period.EndYear = request.EndYear;
            period.IsSingleDayRelease = request.IsSingleDayRelease;
            period.EpisodeCount = request.EpisodeCount;
            period.RottenTomatoesId = RottenTomatoesId.Nullable(request.RottenTomatoesId);
        }
    }

    extension(SpecialEpisode episode)
    {
        private SpecialEpisodeModel ToSpecialEpisodeModel(Series series, IPosterUrlProvider posterUrlProvider) =>
            new(
                episode.Id.Value,
                episode.AllTitles.ToTitleModels(isOriginal: false),
                episode.AllTitles.ToTitleModels(isOriginal: true),
                episode.SequenceNumber,
                episode.IsWatched,
                episode.IsReleased,
                episode.Channel,
                episode.Month,
                episode.Year,
                episode.RottenTomatoesId?.Value,
                posterUrlProvider.GetPosterUrl(series.Id, episode.Id, episode.PosterHash));

        private void Update(SpecialEpisodeRequest request)
        {
            episode.ReplaceTitles(
                request.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: false);

            episode.ReplaceTitles(
                request.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: true);

            episode.Month = request.Month;
            episode.Year = request.Year;
            episode.IsWatched = request.IsWatched;
            episode.IsReleased = request.IsReleased;
            episode.Channel = request.Channel;
            episode.SequenceNumber = request.SequenceNumber;
            episode.RottenTomatoesId = RottenTomatoesId.Nullable(request.RottenTomatoesId);
        }
    }

    extension(SeasonRequest request)
    {
        private Season ToSeason() =>
            new(
                Id.ForNullable<Season>(request.Id),
                request.ToTitles(),
                request.WatchStatus,
                request.ReleaseStatus,
                request.Channel,
                request.SequenceNumber,
                request.Periods.Select(ToPeriod));
    }

    extension(PeriodRequest request)
    {
        private Period ToPeriod() =>
            new(
                Id.ForNullable<Period>(request.Id),
                request.StartMonth,
                request.StartYear,
                request.EndMonth,
                request.EndYear,
                request.IsSingleDayRelease,
                request.EpisodeCount)
            {
                RottenTomatoesId = RottenTomatoesId.Nullable(request.RottenTomatoesId)
            };
    }

    extension(SpecialEpisodeRequest request)
    {
        private SpecialEpisode ToSpecialEpisode() =>
            new(
                Id.ForNullable<SpecialEpisode>(request.Id),
                request.ToTitles(),
                request.Month,
                request.Year,
                request.IsWatched,
                request.IsReleased,
                request.Channel,
                request.SequenceNumber)
            {
                RottenTomatoesId = RottenTomatoesId.Nullable(request.RottenTomatoesId)
            };
    }
}
