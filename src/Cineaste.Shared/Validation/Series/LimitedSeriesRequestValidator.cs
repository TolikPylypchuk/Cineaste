using static Cineaste.Common.Constants;

namespace Cineaste.Shared.Validation.Series;

public sealed class LimitedSeriesRequestValidator : TitledRequestValidator<LimitedSeriesRequest>
{
    public LimitedSeriesRequestValidator()
        : base("LimitedSeries")
    {
        this.RuleFor(req => req.Period)
            .SetValidator(ReleasePeriodRequest.Validator);

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

        this.RuleFor(req => req.Channel)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Channel, Empty))
            .MaximumLength(MaxNameLength)
            .WithErrorCode(this.ErrorCode(req => req.Channel, TooLong));

        this.RuleFor(req => req.ImdbId)
            .Matches(ImdbIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.ImdbId, Invalid));

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));

        this.RuleFor(req => req.RottenTomatoesSubId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesSubId, Invalid));
    }
}
