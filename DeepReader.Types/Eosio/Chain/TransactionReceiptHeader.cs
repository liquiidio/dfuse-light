using DeepReader.Types.Enums;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceiptHeader : IEosioSerializable<TransactionReceiptHeader>
{
    public TransactionStatus Status;    // fc::enum_type<uint8_t,status_enum> v

    public uint CpuUsageUs;

    public uint NetUsageWords;

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
        NetUsageWords = (uint)reader.Read7BitEncodedInt();
    }

    public static TransactionReceiptHeader ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new TransactionReceiptHeader(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Status);
        writer.Write(CpuUsageUs);
        writer.Write7BitEncodedInt((int)NetUsageWords);
    }
}