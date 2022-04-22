using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceipt : TransactionReceiptHeader, IEosioSerializable<TransactionReceipt>
{
    [SortOrder(4)]
    public TransactionVariant Trx;

    public TransactionReceipt(BinaryReader reader) : base(reader)
    {
        Trx = TransactionVariant.ReadFromBinaryReader(reader);
    }
    public new static TransactionReceipt ReadFromBinaryReader(BinaryReader reader)
    {
        return new TransactionReceipt(reader);
    }
}