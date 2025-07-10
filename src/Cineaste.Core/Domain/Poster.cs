namespace Cineaste.Core.Domain;

public sealed record Poster
{
    public byte[] RawData { get; }

    public Poster(byte[] rawData) =>
        this.RawData = Require.NotNull(rawData).Length != 0
            ? rawData
            : throw new ArgumentOutOfRangeException(nameof(rawData));
}
