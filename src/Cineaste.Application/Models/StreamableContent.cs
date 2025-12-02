namespace Cineaste.Application.Models;

public sealed class StreamableContent(Func<Stream> getStream, long length, string type)
{
    public long Length { get; } = length;
    public string Type { get; } = type;

    public Stream GetStream() =>
        getStream();

    public async Task<BinaryContent> ReadDataAsync(CancellationToken token = default)
    {
        var data = new byte[this.Length];

        using var dataStream = getStream();
        await dataStream.ReadExactlyAsync(data, 0, data.Length, token);

        return new BinaryContent(data, this.Type);
    }
}
