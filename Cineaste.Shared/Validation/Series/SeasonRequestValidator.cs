namespace Cineaste.Shared.Validation.Series;

using static Cineaste.Basic.Constants;

public sealed class SeasonRequestValidator : TitledRequestValidator<SeasonRequest>
{
    private static readonly IValidator<PeriodRequest> periodValidator = PeriodRequest.CreateValidator();

    public SeasonRequestValidator()
        : base("Season")
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
                SeasonReleaseStatus.NotStarted => x.WatchStatus is SeasonWatchStatus.NotWatched,
                SeasonReleaseStatus.Running or SeasonReleaseStatus.Hiatus => x.WatchStatus is
                    SeasonWatchStatus.NotWatched or SeasonWatchStatus.Watching or SeasonWatchStatus.Hiatus,
                SeasonReleaseStatus.Finished => x.WatchStatus is
                    SeasonWatchStatus.NotWatched or SeasonWatchStatus.Watching or
                    SeasonWatchStatus.Hiatus or SeasonWatchStatus.Watched,
                SeasonReleaseStatus.Unknown => x.WatchStatus is SeasonWatchStatus.StoppedWatching,
                _ => true
            })
            .WithErrorCode(this.ErrorCode(req => req.WatchStatus, Invalid));

        this.RuleFor(req => req.Channel)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Channel, Empty))
            .MaximumLength(MaxNameLength)
            .WithErrorCode(this.ErrorCode(req => req.Channel, TooLong));

        this.RuleFor(req => req.Periods)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Periods, Empty))
            .Must(NotOverlap)
            .WithErrorCode(this.ErrorCode(req => req.Periods, Overlap));

        this.RuleForEach(req => req.Periods)
            .SetValidator(periodValidator);
    }

    private static bool NotOverlap(IEnumerable<PeriodRequest> periods) =>
        periods
            .OrderBy(period => period.StartYear)
            .ThenBy(period => period.StartMonth)
            .ThenBy(period => period.EndYear)
            .ThenBy(period => period.EndMonth)
            .Buffer(2, 1)
            .Where(periods => periods.Count == 2)
            .All(periods => periods[0].EndYear < periods[1].StartYear ||
                periods[0].EndYear == periods[1].StartYear && periods[0].EndMonth < periods[1].StartMonth);
}
