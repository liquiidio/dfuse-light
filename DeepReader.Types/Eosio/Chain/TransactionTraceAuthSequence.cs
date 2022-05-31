using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// flat_map<account_name,uint64_t>
/// </summary>
public class TransactionTraceAuthSequence : IEosioSerializable<TransactionTraceAuthSequence>
{
    public Name Account { get; set; }
    public ulong Sequence { get; set; }

    public TransactionTraceAuthSequence(BinaryReader reader)
    {
        Account = reader.ReadName();
        Sequence = reader.ReadUInt64();
    }

    public static TransactionTraceAuthSequence ReadFromBinaryReader(BinaryReader reader)
    {
        return new TransactionTraceAuthSequence(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Account);
        writer.Write(Sequence);
    }
}