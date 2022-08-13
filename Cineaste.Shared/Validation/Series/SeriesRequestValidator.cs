namespace Cineaste.Shared.Validation.Series;

using static Cineaste.Basic.Constants;

public sealed class SeriesRequestValidator : TitledRequestValidator<SeriesRequest>
{
    private static readonly IValidator<SeasonRequest> seasonValidator = SeasonRequest.CreateValidator();
    private static readonly IValidator<SpecialEpisodeRequest> specialEpisodeValidator =
        SpecialEpisodeRequest.CreateValidator();

    public SeriesRequestValidator()
        : base("Series")
    {
        this.RuleFor(req => req.WatchStatus)
            .IsInEnum()
            .WithErrorCode(this.ErrorCode(req => req.WatchStatus, Invalid));

        this.RuleFor(req => req.ReleaseStatus)
            .IsInEnum()
            .WithErrorCode(this.ErrorCode(req => req.ReleaseStatus, Invalid));

        this.RuleFor(req => new { req.WatchStatus, req.ReleaseStatus })
            .Must(x => x.ReleaseStatus switch
            {
                SeriesReleaseStatus.NotStarted => x.WatchStatus is SeriesWatchStatus.NotWatched,
                SeriesReleaseStatus.Running or SeriesReleaseStatus.Hiatus => x.WatchStatus is
                    SeriesWatchStatus.NotWatched or SeriesWatchStatus.Watching or SeriesWatchStatus.Hiatus,
                SeriesReleaseStatus.Finished or SeriesReleaseStatus.Cancelled => x.WatchStatus is
                    SeriesWatchStatus.NotWatched or SeriesWatchStatus.Watching or
                    SeriesWatchStatus.Hiatus or SeriesWatchStatus.Watched,
                SeriesReleaseStatus.Unknown => x.WatchStatus is SeriesWatchStatus.StoppedWatching,
                _ => true
            })
            .WithErrorCode(this.ErrorCode(req => req.WatchStatus, Invalid));

        this.RuleFor(req => req.Seasons)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Seasons, Empty));

        this.RuleForEach(req => req.Seasons)
            .SetValidator(seasonValidator);

        this.RuleForEach(req => req.SpecialEpisodes)
            .SetValidator(specialEpisodeValidator);

        this.RuleFor(req => new { req.IsMiniseries, req.Seasons, req.SpecialEpisodes })
            .Must(x => x.IsMiniseries.Implies(
                x.Seasons.Count == 1 && x.Seasons[0].Periods.Count == 1 && x.SpecialEpisodes.Count == 0))
            .WithErrorCode(this.ErrorCode(req => req.IsMiniseries, Invalid));

        this.RuleFor(req => req.ImdbId)
            .Matches(ImdbIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.ImdbId, Invalid));

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));
    }
}
