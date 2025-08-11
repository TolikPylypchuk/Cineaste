using System.Security.Cryptography;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cineaste.Persistence.Configuration;

internal sealed class PosterHashConverter : ValueConverter<PosterHash, string>
{
    public PosterHashConverter()
        : base(
            posterHash => posterHash.Value,
            hash => new PosterHash(hash),
            new ConverterMappingHints(SHA256.HashSizeInBytes * 2))
    { }
}
