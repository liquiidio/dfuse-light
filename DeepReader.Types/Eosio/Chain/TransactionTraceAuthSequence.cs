using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// flat_map<account_name,uint64_t>
/// </summary>
public class TransactionTraceAuthSequence : IEosioSerializable<TransactionTraceAuthSequence>
{
    public Name Account = string.Empty;
    public ulong Sequence = 0;

    public static TransactionTraceAuthSequence ReadFromBinaryReader(BinaryReader reader)
    {
        var seq = new TransactionTraceAuthSequence()
        {
            Account = reader.ReadName(),
            Sequence = reader.ReadUInt64()
        };
        return seq;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Account);
        writer.Write(Sequence);
    }
}