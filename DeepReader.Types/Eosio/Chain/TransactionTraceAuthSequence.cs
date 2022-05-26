using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// flat_map<account_name,uint64_t>
/// </summary>
public sealed class TransactionTraceAuthSequence : IEosioSerializable<TransactionTraceAuthSequence>
{
    public Name Account;
    public ulong Sequence;

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