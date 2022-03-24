using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class TransactionHeader : IEosioSerializable<TransactionHeader>
{
    // abi-field-name: expiration ,abi-field-type: time_point_sec
    [SortOrder(1)]
    [JsonPropertyName("expiration")]
    public Timestamp Expiration = Timestamp.Zero;

    // abi-field-name: ref_block_num ,abi-field-type: uint16
    [SortOrder(2)]
    [JsonPropertyName("ref_block_num")]
    public ushort RefBlockNum;

    // abi-field-name: ref_block_prefix ,abi-field-type: uint32
    [SortOrder(3)]
    [JsonPropertyName("ref_block_prefix")]
    public uint RefBlockPrefix;

    // abi-field-name: max_net_usage_words ,abi-field-type: varuint32
    [SortOrder(4)]
    [JsonPropertyName("max_net_usage_words")]
    public VarUint32 MaxNetUsageWords = 0;

    // abi-field-name: max_cpu_usage_ms ,abi-field-type: uint8
    [SortOrder(5)]
    [JsonPropertyName("max_cpu_usage_ms")]
    public byte MaxCpuUsageMs;

    // abi-field-name: delay_sec ,abi-field-type: varuint32
    [SortOrder(6)]
    [JsonPropertyName("delay_sec")]
    public VarUint32 DelaySec = 0;

    public TransactionHeader()
    {
    }

    public TransactionHeader(uint expiration, ushort refBlockNum, uint refBlockPrefix, VarUint32 maxNetUsageWords, byte maxCpuUsageMs, VarUint32 delaySec)
    {
        this.Expiration = expiration;
        this.RefBlockNum = refBlockNum;
        this.RefBlockPrefix = refBlockPrefix;
        this.MaxNetUsageWords = maxNetUsageWords;
        this.MaxCpuUsageMs = maxCpuUsageMs;
        this.DelaySec = delaySec;
    }

    public static TransactionHeader ReadFromBinaryReader(BinaryReader reader)
    {
        var header = new TransactionHeader()
        {
            Expiration = reader.ReadTimestamp(),
            RefBlockNum = reader.ReadUInt16(),
            RefBlockPrefix = reader.ReadUInt32(),
            MaxNetUsageWords = reader.ReadVarUint32Obj(),
            MaxCpuUsageMs = reader.ReadByte(),
            DelaySec = reader.ReadVarUint32Obj()
        };
        return header;
    }
}