using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceipt : TransactionReceiptHeader
{
    [SortOrder(4)]
    public TransactionVariant Trx = TransactionId.Empty;

    public new static TransactionReceipt ReadFromBinaryReader(BinaryReader reader)
    {
        // Todo: (Haron) Finish here once I test casting
        var transactionReceipt = new TransactionReceipt()
        {
            // Todo: @corvin confirm this
            Trx = reader.ReadTransactionId()
        };
        return transactionReceipt;
    }
}