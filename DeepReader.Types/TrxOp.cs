using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public class TrxOp
{
    public TrxOpOperation Operation = TrxOpOperation.UNKNOWN;//TrxOp_Operation
    public Name Name = 0;//string
    public TransactionId TransactionId = TransactionId.Empty;//string
    public SignedTransaction Transaction;//*SignedTransaction
}