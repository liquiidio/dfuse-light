using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class TrxOp
{
    public TrxOpOperation Operation = TrxOpOperation.UNKNOWN;//TrxOp_Operation
    public string Name = string.Empty;//string
    public string TransactionId = string.Empty;//string
    public SignedTransaction Transaction = new SignedTransaction();//*SignedTransaction
}