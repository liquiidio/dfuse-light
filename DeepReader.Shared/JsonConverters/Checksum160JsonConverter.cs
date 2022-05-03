using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.JsonConverters;

internal class Checksum256JsonConverter : JsonConverter<Checksum256>
{
    public override Checksum256 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Checksum256 value, JsonSerializerOptions options)
    {
        // TODO
        writer.WriteStringValue(value.StringVal);
    }

}