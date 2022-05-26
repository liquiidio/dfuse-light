using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class SignedBlock : SignedBlockHeader, IEosioSerializable<SignedBlock>
{
    public TransactionReceipt[] Transactions;

    public Extension[] BlockExtensions;

    public SignedBlock(BinaryReader reader) : base(reader)
    {
        Transactions = new TransactionReceipt[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Transactions.Length; i++)
        {
            Transactions[i] = TransactionReceipt.ReadFromBinaryReader(reader);
        }

        BlockExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i != BlockExtensions.Length; i++)
        {
            BlockExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }
    }

    public new static SignedBlock ReadFromBinaryReader(BinaryReader reader)
    {
        return new SignedBlock(reader);
    }
}