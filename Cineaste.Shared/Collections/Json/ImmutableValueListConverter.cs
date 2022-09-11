namespace Cineaste.Shared.Collections.Json;

using System.Text.Json.Serialization;
using System.Text.Json;

public sealed class ImmutableValueListConverter<T> : JsonConverter<ImmutableValueList<T>>
{
    public override ImmutableValueList<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"{nameof(ImmutableValueList)} can only be deserialized from a JSON array");
        }

        reader.Read();

        var elements = new List<T>();

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            var value = JsonSerializer.Deserialize<T>(ref reader, options);

            if (value is not null)
            {
                elements.Add(value);
            }

            reader.Read();
        }

        return elements.ToImmutableList().AsValue();
    }

    public override void Write(Utf8JsonWriter writer, ImmutableValueList<T> value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value.AsEnumerable(), options);
}
