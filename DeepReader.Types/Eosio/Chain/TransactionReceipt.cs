using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceipt : TransactionReceiptHeader
{
    [SortOrder(4)]
    public TransactionVariant Trx = TransactionId.Empty;

    public static TransactionReceipt ReadFromBinaryReader(BinaryReader reader)
    {
        // Todo(Haron): complete this
        return new();
    }
}