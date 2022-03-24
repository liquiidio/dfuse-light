using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.JsonConverters;

internal class TransactionIdJsonConverter : JsonConverter<TransactionId>
{
    public override TransactionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, TransactionId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringVal);
    }

}