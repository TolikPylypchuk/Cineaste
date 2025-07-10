using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cineaste.Shared.Collections.Json;

public class ImmutableValueListConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(ImmutableValueList<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];

        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(ImmutableValueListConverter<>).MakeGenericType(elementType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converter;
    }
}
