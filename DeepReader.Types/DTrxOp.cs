using System.Text.Json.Serialization;
using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types;

[JsonConverter(typeof(DTrxOpJsonConverter))]
public sealed class DTrxOp
{
    public DTrxOpOperation Operation = DTrxOpOperation.UNKNOWN;//DTrxOp_Operation
    public uint ActionIndex = 0;//uint32
    public Name Sender = Name.TypeEmpty;//string
    [JsonIgnore]
    public ReadOnlyMemory<char> SenderId = ReadOnlyMemory<char>.Empty;//string
    public Name Payer = Name.TypeEmpty;//string
    public DateTimeOffset PublishedAt = default;//string
    public DateTimeOffset DelayUntil = default;//string
    public DateTimeOffset ExpirationAt = default;//string
    public TransactionId TransactionId = TransactionId.TypeEmpty;//string
    public SignedTransaction Transaction;//*SignedTransaction
}