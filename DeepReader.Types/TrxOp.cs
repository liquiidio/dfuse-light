using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public class TrxOp
{
    public TrxOpOperation Operation = TrxOpOperation.UNKNOWN;//TrxOp_Operation
    public Name Name = Name.TypeEmpty;//string
    public TransactionId TransactionId = TransactionId.TypeEmpty;//string
    public SignedTransaction Transaction;//*SignedTransaction
}