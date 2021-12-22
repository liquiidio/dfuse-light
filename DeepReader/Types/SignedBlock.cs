using DeepReader.Types;

namespace DeepReader;

public class SignedBlock : SignedBlockHeader
{
    public TransactionReceipt[] Transactions;//    []TransactionReceipt `json:"transactions"`
    public Extension[] BlockExtensions;// []*Extension         `json:"block_extensions"`
}