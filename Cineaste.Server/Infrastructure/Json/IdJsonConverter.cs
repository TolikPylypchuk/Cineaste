namespace Cineaste.Server.Infrastructure.Json;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class IdJsonConverter<T> : JsonConverter<Id<T>>
{
    private readonly JsonConverter<Guid> guidConverter;

    public IdJsonConverter(JsonSerializerOptions options) =>
        this.guidConverter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

    public override Id<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(this.guidConverter.Read(ref reader, typeof(Guid), options));

    public override void Write(Utf8JsonWriter writer, Id<T> id, JsonSerializerOptions options) =>
        this.guidConverter.Write(writer, id.Value, options);
}
