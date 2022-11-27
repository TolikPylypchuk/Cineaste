namespace Cineaste.Shared.Validation.TestData;

using Cineaste.Basic;

internal class SeasonTestData : TestDataBase
{
    public SeasonTestData()
    {
        // SeasonWatchStatus watchStatus, SeasonReleaseStatus releaseStatus, bool isValid

        this.Add(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.NotStarted, true);
        this.Add(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Running, true);
        this.Add(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Hiatus, true);
        this.Add(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Finished, true);
        this.Add(SeasonWatchStatus.NotWatched, SeasonReleaseStatus.Unknown, false);

        this.Add(SeasonWatchStatus.Watching, SeasonReleaseStatus.NotStarted, false);
        this.Add(SeasonWatchStatus.Watching, SeasonReleaseStatus.Running, true);
        this.Add(SeasonWatchStatus.Watching, SeasonReleaseStatus.Hiatus, true);
        this.Add(SeasonWatchStatus.Watching, SeasonReleaseStatus.Finished, true);
        this.Add(SeasonWatchStatus.Watching, SeasonReleaseStatus.Unknown, false);

        this.Add(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.NotStarted, false);
        this.Add(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Running, true);
        this.Add(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Hiatus, true);
        this.Add(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Finished, true);
        this.Add(SeasonWatchStatus.Hiatus, SeasonReleaseStatus.Unknown, false);

        this.Add(SeasonWatchStatus.Watched, SeasonReleaseStatus.NotStarted, false);
        this.Add(SeasonWatchStatus.Watched, SeasonReleaseStatus.Running, false);
        this.Add(SeasonWatchStatus.Watched, SeasonReleaseStatus.Hiatus, false);
        this.Add(SeasonWatchStatus.Watched, SeasonReleaseStatus.Finished, true);
        this.Add(SeasonWatchStatus.Watched, SeasonReleaseStatus.Unknown, false);

        this.Add(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.NotStarted, false);
        this.Add(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Running, false);
        this.Add(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Hiatus, false);
        this.Add(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Finished, false);
        this.Add(SeasonWatchStatus.StoppedWatching, SeasonReleaseStatus.Unknown, true);
    }
}
