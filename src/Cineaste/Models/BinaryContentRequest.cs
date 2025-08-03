namespace Cineaste.Models;

public sealed class BinaryContentRequest(Func<Stream> openStream, long length, string type)
{
    public string Type { get; } = type;

    public async Task<byte[]> ReadDataAsync(CancellationToken token = default)
    {
        var data = new byte[length];

        using var dataStream = openStream();
        await dataStream.ReadExactlyAsync(data, 0, data.Length, token);

        return data;

    }
}
