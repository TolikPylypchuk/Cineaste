namespace Cineaste.Converters;

public sealed class SeasonReleaseStatusConverter : EnumConverter<SeasonReleaseStatus>
{
    protected override Dictionary<SeasonReleaseStatus, string> CreateConverterDictionary() =>
        new()
        {
            [SeasonReleaseStatus.NotStarted] = Messages.SeasonNotStarted,
            [SeasonReleaseStatus.Running] = Messages.SeasonRunning,
            [SeasonReleaseStatus.Hiatus] = Messages.SeasonHiatus,
            [SeasonReleaseStatus.Finished] = Messages.SeasonFinished,
            [SeasonReleaseStatus.Unknown] = Messages.SeasonUnknown
        };
}
