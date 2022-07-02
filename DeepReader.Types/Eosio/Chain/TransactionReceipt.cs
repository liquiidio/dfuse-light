using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public sealed class TransactionReceipt : TransactionReceiptHeader, IEosioSerializable<TransactionReceipt>
{
    public TransactionVariant Trx;

    public TransactionReceipt(IBufferReader reader) : base(reader)
    {
        Trx = TransactionVariant.ReadFromBinaryReader(reader);
    }
    public new static TransactionReceipt ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new TransactionReceipt(reader);
    }
}