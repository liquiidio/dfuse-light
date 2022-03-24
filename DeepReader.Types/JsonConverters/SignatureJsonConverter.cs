using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.JsonConverters;

internal class SignatureJsonConverter : JsonConverter<Signature>
{
    public override Signature Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Signature value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringVal);
    }

}