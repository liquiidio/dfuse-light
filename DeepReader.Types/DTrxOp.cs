using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Types;

public class DTrxOp
{
    public DTrxOpOperation Operation = DTrxOpOperation.UNKNOWN;//DTrxOp_Operation
    public uint ActionIndex = 0;//uint32
    public string Sender = string.Empty;//string
    public string SenderId = string.Empty;//string
    public string Payer = string.Empty;//string
    public string PublishedAt = string.Empty;//string
    public string DelayUntil = string.Empty;//string
    public string ExpirationAt = string.Empty;//string
    public string TransactionId = string.Empty;//string
    public SignedTransaction Transaction = new();//*SignedTransaction
}