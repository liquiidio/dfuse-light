using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class SignedBlock : SignedBlockHeader, IEosioSerializable<SignedBlock>
{
    [SortOrder(11)]
    public TransactionReceipt[] Transactions = Array.Empty<TransactionReceipt>();
    [SortOrder(12)]
    public Extension[] BlockExtensions = Array.Empty<Extension>();

    public new static SignedBlock ReadFromBinaryReader(BinaryReader reader)
    {
        var signedBlock = (SignedBlock)SignedBlockHeader.ReadFromBinaryReader(reader);
        
        signedBlock.Transactions = new TransactionReceipt[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedBlock.Transactions.Length; i++)
        {
            signedBlock.Transactions[i] = TransactionReceipt.ReadFromBinaryReader(reader);
        }

        signedBlock.BlockExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i != signedBlock.BlockExtensions.Length; i++)
        {
            signedBlock.BlockExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }

        return signedBlock;
    }
}