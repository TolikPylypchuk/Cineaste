using System.Collections;

namespace Cineaste.Shared.Validation.TestData;

internal class SeriesTestData : IEnumerable<TheoryDataRow<SeriesWatchStatus, SeriesReleaseStatus, bool>>
{
    // SeriesWatchStatus watchStatus, SeriesReleaseStatus releaseStatus, bool isValid
    public IEnumerator<TheoryDataRow<SeriesWatchStatus, SeriesReleaseStatus, bool>> GetEnumerator()
    {
        yield return (SeriesWatchStatus.NotWatched, SeriesReleaseStatus.NotStarted, true);
        yield return (SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Running, true);
        yield return (SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Hiatus, true);
        yield return (SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Finished, true);
        yield return (SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Cancelled, true);
        yield return (SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Unknown, false);

        yield return (SeriesWatchStatus.Watching, SeriesReleaseStatus.NotStarted, false);
        yield return (SeriesWatchStatus.Watching, SeriesReleaseStatus.Running, true);
        yield return (SeriesWatchStatus.Watching, SeriesReleaseStatus.Hiatus, true);
        yield return (SeriesWatchStatus.Watching, SeriesReleaseStatus.Finished, true);
        yield return (SeriesWatchStatus.Watching, SeriesReleaseStatus.Cancelled, true);
        yield return (SeriesWatchStatus.Watching, SeriesReleaseStatus.Unknown, false);

        yield return (SeriesWatchStatus.Hiatus, SeriesReleaseStatus.NotStarted, false);
        yield return (SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Running, true);
        yield return (SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Hiatus, true);
        yield return (SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Finished, true);
        yield return (SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Cancelled, true);
        yield return (SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Unknown, false);

        yield return (SeriesWatchStatus.Watched, SeriesReleaseStatus.NotStarted, false);
        yield return (SeriesWatchStatus.Watched, SeriesReleaseStatus.Running, false);
        yield return (SeriesWatchStatus.Watched, SeriesReleaseStatus.Hiatus, false);
        yield return (SeriesWatchStatus.Watched, SeriesReleaseStatus.Finished, true);
        yield return (SeriesWatchStatus.Watched, SeriesReleaseStatus.Cancelled, true);
        yield return (SeriesWatchStatus.Watched, SeriesReleaseStatus.Unknown, false);

        yield return (SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.NotStarted, false);
        yield return (SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Running, false);
        yield return (SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Hiatus, false);
        yield return (SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Finished, false);
        yield return (SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Cancelled, false);
        yield return (SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Unknown, true);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
