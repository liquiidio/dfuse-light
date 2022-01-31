namespace DeepReader.Types;

public class SignedBlock : SignedBlockHeader
{
    public byte PruneState = 0;
    public TransactionReceipt[] Transactions = Array.Empty<TransactionReceipt>();//[]TransactionReceipt
    public Extension[] BlockExtensions = Array.Empty<Extension>();//[]*Extension
}