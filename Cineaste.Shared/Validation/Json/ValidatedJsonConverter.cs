namespace Cineaste.Shared.Validation.Json;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class ValidatedJsonConverter<T> : JsonConverter<Validated<T>>
    where T : IValidatable<T>
{
    private readonly JsonConverter<T> valueConverter;

    public ValidatedJsonConverter(JsonSerializerOptions options) =>
        this.valueConverter = (JsonConverter<T>)options.GetConverter(typeof(T));

    public override Validated<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        typeToConvert.IsGenericType
            ? this.valueConverter.Read(ref reader, typeToConvert.GetGenericArguments()[0], options).Validated()
            : null;

    public override void Write(Utf8JsonWriter writer, Validated<T> value, JsonSerializerOptions options) =>
        this.valueConverter.Write(writer, value.Value, options);
}
