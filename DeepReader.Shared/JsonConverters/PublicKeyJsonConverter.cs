using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.JsonConverters;

internal class PublicKeyJsonConverter : JsonConverter<PublicKey>
{
    public override PublicKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, PublicKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}