using System.Text.Json.Serialization;

namespace Cineaste.Server.Infrastructure.Json;

internal sealed class IdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Id<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        this.CanConvert(typeToConvert)
            ? (JsonConverter?)Activator.CreateInstance(
                typeof(IdJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]), options)
            : null;
}
