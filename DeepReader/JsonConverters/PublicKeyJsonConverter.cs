using DeepReader.EosTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepReader.JsonConverters
{
    internal class PublicKeyJsonConverter : JsonConverter<PublicKey>
    {
        public override PublicKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new PublicKey(reader.GetString() ?? "");
        }

        public override void Write(Utf8JsonWriter writer, PublicKey value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
