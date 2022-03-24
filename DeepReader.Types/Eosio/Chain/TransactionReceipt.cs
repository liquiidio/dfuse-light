using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceipt : TransactionReceiptHeader, IEosioSerializable<TransactionReceipt>
{
    [SortOrder(4)]
    public TransactionVariant Trx = TransactionId.Empty;

    public new static TransactionReceipt ReadFromBinaryReader(BinaryReader reader)
    {
        var transactionReceipt = new TransactionReceipt()
        {
            Status = reader.ReadByte(),
            CpuUsageUs = reader.ReadUInt32(),
            NetUsageWords = reader.ReadVarUint32Obj()
        }; 
        transactionReceipt.Trx = TransactionVariant.ReadFromBinaryReader(reader);
        return transactionReceipt;
    }
}