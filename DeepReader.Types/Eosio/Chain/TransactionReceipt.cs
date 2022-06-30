using Salar.BinaryBuffers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public sealed class TransactionReceipt : TransactionReceiptHeader, IEosioSerializable<TransactionReceipt>
{
    public TransactionVariant Trx;

    public TransactionReceipt(BinaryBufferReader reader) : base(reader)
    {
        Trx = TransactionVariant.ReadFromBinaryReader(reader);
    }
    public new static TransactionReceipt ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new TransactionReceipt(reader);
    }
}