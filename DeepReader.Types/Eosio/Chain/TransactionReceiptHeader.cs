using DeepReader.Types.Helpers;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceiptHeader : IEosioSerializable<TransactionReceiptHeader>
{
    [SortOrder(1)]
    public byte Status;    // fc::enum_type<uint8_t,status_enum> v
    [SortOrder(2)]
    public uint CpuUsageUs;
    [SortOrder(3)]
    public VarUint32 NetUsageWords = 0;

    public static TransactionReceiptHeader ReadFromBinaryReader(BinaryReader reader)
    {
        var transactionReceiptHeader = new TransactionReceiptHeader()
        {
            Status = reader.ReadByte(),
            CpuUsageUs = reader.ReadUInt32(),
            NetUsageWords = reader.ReadVarUint32Obj()
        };
        return transactionReceiptHeader;
    }
}