using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public sealed class SignedBlock : SignedBlockHeader, IEosioSerializable<SignedBlock>
{
    public TransactionReceipt[] Transactions;

    public Extension[] BlockExtensions;

    public SignedBlock(IBufferReader reader) : base(reader)
    {
        Transactions = new TransactionReceipt[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Transactions.Length; i++)
        {
            Transactions[i] = TransactionReceipt.ReadFromBinaryReader(reader);
        }

        BlockExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i != BlockExtensions.Length; i++)
        {
            BlockExtensions[i] = reader.ReadExtension();
        }
    }

    public new static SignedBlock ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new SignedBlock(reader);
    }
}