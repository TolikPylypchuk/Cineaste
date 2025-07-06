using static Cineaste.Basic.Constants;

namespace Cineaste.Shared.Validation.Series;

public sealed class SeriesRequestValidator : TitledRequestValidator<SeriesRequest>
{
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

        this.RuleFor(req => new { req.Seasons, req.SpecialEpisodes })
            .Must(x => Enumerable.Concat(
                x.Seasons.Select(season => season.SequenceNumber),
                x.SpecialEpisodes.Select(episode => episode.SequenceNumber))
                .OrderBy(num => num)
                .Select((num, index) => num == index + 1)
                .All(isValid => isValid))
            .WithErrorCode(this.ErrorCode(Sequence, Invalid));

        this.RuleForEach(req => req.Seasons)
            .SetValidator(SeasonRequest.Validator);

        this.RuleForEach(req => req.SpecialEpisodes)
            .SetValidator(SpecialEpisodeRequest.Validator);

        this.RuleFor(req => req.ImdbId)
            .Matches(ImdbIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.ImdbId, Invalid));

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));
    }
}
