using System.Collections;

namespace Cineaste.Shared.Validation.TestData;

internal class SeasonTestData : IEnumerable<TheoryDataRow<SeasonWatchStatus, SeasonReleaseStatus, bool>>
{
    // SeasonWatchStatus watchStatus, SeasonReleaseStatus releaseStatus, bool isValid
    public IEnumerator<TheoryDataRow<SeasonWatchStatus, SeasonReleaseStatus, bool>> GetEnumerator()
    {
        yield return new(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.NotStarted, true);
        yield return new(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Running, true);
        yield return new(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Hiatus, true);
        yield return new(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Finished, true);
        yield return new(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Unknown, false);

        yield return new(SeasonWatchStatus.Watching, SeasonReleaseStatus.NotStarted, false);
        yield return new(SeasonWatchStatus.Watching, SeasonReleaseStatus.Running, true);
        yield return new(SeasonWatchStatus.Watching, SeasonReleaseStatus.Hiatus, true);
        yield return new(SeasonWatchStatus.Watching, SeasonReleaseStatus.Finished, true);
        yield return new(SeasonWatchStatus.Watching, SeasonReleaseStatus.Unknown, false);

        yield return new(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.NotStarted, false);
        yield return new(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Running, true);
        yield return new(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Hiatus, true);
        yield return new(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Finished, true);
        yield return new(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Unknown, false);

        yield return new(SeasonWatchStatus.Watched, SeasonReleaseStatus.NotStarted, false);
        yield return new(SeasonWatchStatus.Watched, SeasonReleaseStatus.Running, false);
        yield return new(SeasonWatchStatus.Watched, SeasonReleaseStatus.Hiatus, false);
        yield return new(SeasonWatchStatus.Watched, SeasonReleaseStatus.Finished, true);
        yield return new(SeasonWatchStatus.Watched, SeasonReleaseStatus.Unknown, false);

        yield return new(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.NotStarted, false);
        yield return new(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Running, false);
        yield return new(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Hiatus, false);
        yield return new(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Finished, false);
        yield return new(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Unknown, true);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
