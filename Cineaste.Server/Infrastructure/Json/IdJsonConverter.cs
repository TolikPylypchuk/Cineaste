namespace Cineaste.Server.Infrastructure.Json;

using System.Text.Json.Serialization;

internal sealed class IdJsonConverter<T> : JsonConverter<Id<T>>
{
    private readonly JsonConverter<Guid> valueConverter;

    public IdJsonConverter(JsonSerializerOptions options) =>
        this.valueConverter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

    public override Id<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        typeToConvert.IsGenericType && this.CreateInstance(ref reader, typeToConvert, options) is Id<T> id
            ? id
            : Id.Create<T>(Guid.Empty);

    public override void Write(Utf8JsonWriter writer, Id<T> id, JsonSerializerOptions options) =>
        this.valueConverter.Write(writer, id.Value, options);

    private object? CreateInstance(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Activator.CreateInstance(typeToConvert, this.valueConverter.Read(ref reader, typeof(Guid), options));
}
