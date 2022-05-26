using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.JsonConverters;

public sealed class DTrxOpJsonConverter : JsonConverter<DTrxOp>
{
    public override DTrxOp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, DTrxOp value, JsonSerializerOptions options)
    {
        // TODO, just a temporary workaround
        writer.WriteStartObject();
        writer.WriteString("operation", value.Operation.ToString());
        writer.WriteNumber("actionIndex", value.ActionIndex);
        writer.WriteString("sender", value.Sender);
        writer.WriteString("payer", value.Payer);
        writer.WriteString("publishedAt", value.PublishedAt);
        writer.WriteString("delayUntil", value.DelayUntil);
        writer.WriteString("expirationAt", value.ExpirationAt);
        writer.WriteString("expirationAt", value.ExpirationAt);
        writer.WriteString("transactionId", value.TransactionId);
        writer.WriteString("transaction", JsonSerializer.Serialize(value.Transaction));
        writer.WriteEndObject();
    }
}