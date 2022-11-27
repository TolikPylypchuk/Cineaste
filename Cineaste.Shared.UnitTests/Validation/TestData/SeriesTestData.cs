namespace Cineaste.Shared.Validation.TestData;

using Cineaste.Basic;

internal class SeriesTestData : TestDataBase
{
    public SeriesTestData()
    {
        // SeriesWatchStatus watchStatus, SeriesReleaseStatus releaseStatus, bool isValid

        this.Add(SeriesWatchStatus.NotWatched, SeriesReleaseStatus.NotStarted, true);
        this.Add(SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Running, true);
        this.Add(SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Hiatus, true);
        this.Add(SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Finished, true);
        this.Add(SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Cancelled, true);
        this.Add(SeriesWatchStatus.NotWatched, SeriesReleaseStatus.Unknown, false);

        this.Add(SeriesWatchStatus.Watching, SeriesReleaseStatus.NotStarted, false);
        this.Add(SeriesWatchStatus.Watching, SeriesReleaseStatus.Running, true);
        this.Add(SeriesWatchStatus.Watching, SeriesReleaseStatus.Hiatus, true);
        this.Add(SeriesWatchStatus.Watching, SeriesReleaseStatus.Finished, true);
        this.Add(SeriesWatchStatus.Watching, SeriesReleaseStatus.Cancelled, true);
        this.Add(SeriesWatchStatus.Watching, SeriesReleaseStatus.Unknown, false);

        this.Add(SeriesWatchStatus.Hiatus, SeriesReleaseStatus.NotStarted, false);
        this.Add(SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Running, true);
        this.Add(SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Hiatus, true);
        this.Add(SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Finished, true);
        this.Add(SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Cancelled, true);
        this.Add(SeriesWatchStatus.Hiatus, SeriesReleaseStatus.Unknown, false);

        this.Add(SeriesWatchStatus.Watched, SeriesReleaseStatus.NotStarted, false);
        this.Add(SeriesWatchStatus.Watched, SeriesReleaseStatus.Running, false);
        this.Add(SeriesWatchStatus.Watched, SeriesReleaseStatus.Hiatus, false);
        this.Add(SeriesWatchStatus.Watched, SeriesReleaseStatus.Finished, true);
        this.Add(SeriesWatchStatus.Watched, SeriesReleaseStatus.Cancelled, true);
        this.Add(SeriesWatchStatus.Watched, SeriesReleaseStatus.Unknown, false);

        this.Add(SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.NotStarted, false);
        this.Add(SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Running, false);
        this.Add(SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Hiatus, false);
        this.Add(SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Finished, false);
        this.Add(SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Cancelled, false);
        this.Add(SeriesWatchStatus.StoppedWatching, SeriesReleaseStatus.Unknown, true);
    }
}
