using DeepReader.Types.Enums;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceiptHeader : IEosioSerializable<TransactionReceiptHeader>, IFasterSerializable<TransactionReceiptHeader>
{

    public TransactionStatus Status { get; set; }    // fc::enum_type<uint8_t,status_enum> v
    public uint CpuUsageUs { get; set; }
    public uint NetUsageWords { get; set; }

    public TransactionReceiptHeader()
    {
        Status = 0;
        CpuUsageUs = 0;
        NetUsageWords = 0;
    }

    public TransactionReceiptHeader(BinaryBufferReader reader)
    {
        Status = (TransactionStatus)reader.ReadByte();
        CpuUsageUs = reader.ReadUInt32();
        NetUsageWords = (uint)reader.Read7BitEncodedInt();
    }

    public static TransactionReceiptHeader ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new TransactionReceiptHeader(reader);
    }

    public static TransactionReceiptHeader ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        return new TransactionReceiptHeader()
        {
            Status = (TransactionStatus)reader.ReadByte(),
            CpuUsageUs = reader.ReadUInt32(),
            NetUsageWords = (uint)reader.Read7BitEncodedInt()
        };
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        writer.Write((byte)Status);
        writer.Write(CpuUsageUs);
        writer.Write7BitEncodedInt((int)NetUsageWords);
    }
}