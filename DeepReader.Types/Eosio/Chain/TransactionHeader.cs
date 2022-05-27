using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class TransactionHeader : IEosioSerializable<TransactionHeader>
{
    // abi-field-name: expiration ,abi-field-type: time_point_sec
    [JsonPropertyName("expiration")]
    public Timestamp Expiration;

    // abi-field-name: ref_block_num ,abi-field-type: uint16
    [JsonPropertyName("ref_block_num")]
    public ushort RefBlockNum;

    // abi-field-name: ref_block_prefix ,abi-field-type: uint32
    [JsonPropertyName("ref_block_prefix")]
    public uint RefBlockPrefix;

    // abi-field-name: max_net_usage_words ,abi-field-type: varuint32
    [JsonPropertyName("max_net_usage_words")]
    public uint MaxNetUsageWords;

    // abi-field-name: max_cpu_usage_ms ,abi-field-type: uint8
    [JsonPropertyName("max_cpu_usage_ms")]
    public byte MaxCpuUsageMs;

    // abi-field-name: delay_sec ,abi-field-type: varuint32
    [JsonPropertyName("delay_sec")]
    public uint DelaySec;

    public TransactionHeader(BinaryReader reader)
    {
        Expiration = Timestamp.ReadFromBinaryReader(reader);
        RefBlockNum = reader.ReadUInt16();
        RefBlockPrefix = reader.ReadUInt32();
        MaxNetUsageWords = (uint)reader.Read7BitEncodedInt();
        MaxCpuUsageMs = reader.ReadByte();
        DelaySec = (uint)reader.Read7BitEncodedInt();
    }

    public static TransactionHeader ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new TransactionHeader(reader);
    }
}