using DeepReader.Types.Enums;
using DeepReader.Types.Extensions;
using DeepReader.Types.Helpers;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceiptHeader : IEosioSerializable<TransactionReceiptHeader>
{
    public TransactionStatus Status { get; set; }    // fc::enum_type<uint8_t,status_enum> v
    public uint CpuUsageUs { get; set; }
    public VarUint32 NetUsageWords { get; set; }

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

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Status);
        writer.Write(CpuUsageUs);
        writer.Write7BitEncodedInt((int)NetUsageWords.Value);
    }
}