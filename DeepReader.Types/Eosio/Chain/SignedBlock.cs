using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class SignedBlock : SignedBlockHeader
{
    [SortOrder(11)]
    public TransactionReceipt[] Transactions = Array.Empty<TransactionReceipt>();
    [SortOrder(12)]
    public Extension[] BlockExtensions = Array.Empty<Extension>();

    public new static SignedBlock ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = (SignedBlock)SignedBlockHeader.ReadFromBinaryReader(reader);
        
        obj.Transactions = new TransactionReceipt[reader.ReadInt32()];
        for (int i = 0; i < obj.Transactions.Length; i++)
        {
            obj.Transactions[i] = TransactionReceipt.ReadFromBinaryReader(reader);
        }

        obj.BlockExtensions = new Extension[reader.ReadInt32()];
        for (int i = 0; i != obj.BlockExtensions.Length; i++)
        {
            obj.BlockExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.ReadInt32()));
        }

        return obj;
    }
}