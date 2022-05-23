using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;

namespace DeepReader.Types.JsonConverters;

public class PublicKeyJsonConverter : JsonConverter<PublicKey>
{
    public override PublicKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var pubKeyString = reader.GetString();
        if(pubKeyString != null)
        {
            //if (pubKeyString.StartsWith("EOS"))
            //    pubKeyString = pubKeyString.Substring(3);

            // TODO this doesn't seem to be right
            return new PublicKey() { Binary = SerializationHelper.PrimaryKeyToBytes(pubKeyString), StringVal = pubKeyString};
        }

        return new PublicKey();
    }

    public override void Write(Utf8JsonWriter writer, PublicKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringVal);
    }
}