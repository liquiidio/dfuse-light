using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.JsonConverters;

internal class Checksum256JsonConverter : JsonConverter<Checksum256>
{
    public override Checksum256 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Checksum256(reader.GetString() ?? "");
    }

    public override void Write(Utf8JsonWriter writer, Checksum256 value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

}