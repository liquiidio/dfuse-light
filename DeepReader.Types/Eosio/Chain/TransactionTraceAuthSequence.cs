using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;
using Salar.BinaryBuffers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// flat_map<account_name,uint64_t>
/// </summary>
public sealed class TransactionTraceAuthSequence : PooledObject<TransactionTraceAuthSequence>, IEosioSerializable<TransactionTraceAuthSequence>, IFasterSerializable<TransactionTraceAuthSequence>
{
    public Name Account { get; set; }
    public ulong Sequence { get; set; }

    public TransactionTraceAuthSequence()
    {
        Account = Name.TypeEmpty;
        Sequence = 0;
    }

    public static TransactionTraceAuthSequence ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new TransactionTraceAuthSequence();

        obj.Account = Name.ReadFromBinaryReader(reader);
        obj.Sequence = reader.ReadUInt64();

        return obj;
    }

    public static TransactionTraceAuthSequence ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new TransactionTraceAuthSequence();

        obj.Account = Name.ReadFromFaster(reader);
        obj.Sequence = reader.ReadUInt64();

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        Account.WriteToFaster(writer);
        writer.Write(Sequence);
    }
}