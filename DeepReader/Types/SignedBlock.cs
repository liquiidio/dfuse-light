namespace DeepReader.Types;

public class SignedBlock : SignedBlockHeader
{
    public TransactionReceipt[] Transactions = Array.Empty<TransactionReceipt>();//[]TransactionReceipt
    public Extension[] BlockExtensions = Array.Empty<Extension>();//[]*Extension
}