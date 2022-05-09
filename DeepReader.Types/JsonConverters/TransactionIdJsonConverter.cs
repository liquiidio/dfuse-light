using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Types.JsonConverters;

public class TransactionIdJsonConverter : JsonConverter<TransactionId>
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