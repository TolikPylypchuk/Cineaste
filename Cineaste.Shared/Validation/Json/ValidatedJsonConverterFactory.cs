using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cineaste.Shared.Validation.Json;

public sealed class ValidatedJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Validated<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        this.CanConvert(typeToConvert)
            ? (JsonConverter?)Activator.CreateInstance(
                typeof(ValidatedJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]), options)
            : null;
}
