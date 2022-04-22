using DeepReader.Types.Enums;
using DeepReader.Types.Helpers;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceiptHeader : IEosioSerializable<TransactionReceiptHeader>
{
    [SortOrder(1)]
    public TransactionStatus Status;    // fc::enum_type<uint8_t,status_enum> v
    [SortOrder(2)]
    public uint CpuUsageUs;
    [SortOrder(3)]
    public VarUint32 NetUsageWords;

    public TransactionReceiptHeader()
    {
        Status = 0;
        CpuUsageUs = 0;
        NetUsageWords = 0;
    }

    public TransactionReceiptHeader(BinaryReader reader)
    {
        Status = (TransactionStatus)reader.ReadByte();
        CpuUsageUs = reader.ReadUInt32();
        NetUsageWords = reader.ReadVarUint32Obj();
    }

    public static TransactionReceiptHeader ReadFromBinaryReader(BinaryReader reader)
    {
        return new TransactionReceiptHeader(reader);
    }
}