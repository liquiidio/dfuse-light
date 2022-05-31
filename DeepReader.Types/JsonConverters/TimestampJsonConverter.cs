using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.JsonConverters;

public sealed class TimestampJsonConverter : JsonConverter<Timestamp>
{
    public override Timestamp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Timestamp((uint)DateTimeOffset.Parse(reader.GetString() ?? "").ToUnixTimeSeconds());
    }

    public override void Write(Utf8JsonWriter writer, Timestamp value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToDateTime().ToString(CultureInfo.InvariantCulture));
    }

}