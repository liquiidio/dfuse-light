using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public class DTrxOp
{
    public DTrxOpOperation Operation = DTrxOpOperation.UNKNOWN;//DTrxOp_Operation
    public uint ActionIndex = 0;//uint32
    public Name Sender = Name.Empty;//string
    public ulong SenderId;//string
    public Name Payer = Name.Empty;//string
    public DateTimeOffset PublishedAt = default;//string
    public DateTimeOffset DelayUntil = default;//string
    public DateTimeOffset ExpirationAt = default;//string
    public TransactionId TransactionId = TransactionId.Empty;//string
    public SignedTransaction Transaction;//*SignedTransaction
}