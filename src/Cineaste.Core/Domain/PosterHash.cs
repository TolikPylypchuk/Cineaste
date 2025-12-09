using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Cineaste.Core.Domain;

public readonly partial record struct PosterHash
{
    private static readonly Regex Sha256Regex = GenerateSha256Regex();

    public string Value { get; }

    public PosterHash(string value) =>
        this.Value = Sha256Regex.IsMatch(Require.NotNull(value))
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));

    public static PosterHash ForPoster(byte[] poster) =>
        poster is not null
            ? new(Convert.ToHexStringLower(SHA256.HashData(poster)))
            : throw new ArgumentNullException(nameof(poster));

    public static PosterHash? Nullable(string? value) =>
        value is not null ? new(value) : null;

    public override string ToString() =>
        this.Value;

    [GeneratedRegex("^[0-9a-f]{64}$", RegexOptions.Compiled)]
    private static partial Regex GenerateSha256Regex();
}
