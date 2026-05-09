using static Cineaste.Common.Constants;

namespace Cineaste.Shared.Validation.Series;

public sealed class SeasonRequestValidator : TitledRequestValidator<SeasonRequest>
{
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

        this.RuleFor(req => req.Parts)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Parts, Empty))
            .Must(NotOverlap)
            .WithErrorCode(this.ErrorCode(req => req.Parts, Overlap));

        this.RuleForEach(req => req.Parts)
            .SetValidator(SeasonPartRequest.Validator);
    }

    private static bool NotOverlap(IEnumerable<SeasonPartRequest> seasonParts) =>
        seasonParts
            .OrderBy(period => period.Period.StartYear)
            .ThenBy(period => period.Period.StartMonth)
            .ThenBy(period => period.Period.EndYear)
            .ThenBy(period => period.Period.EndMonth)
            .Buffer(2, 1)
            .Where(periods => periods.Count == 2)
            .All(parts => parts[0].Period.EndYear < parts[1].Period.StartYear ||
                parts[0].Period.EndYear == parts[1].Period.StartYear &&
                parts[0].Period.EndMonth <= parts[1].Period.StartMonth);
}
