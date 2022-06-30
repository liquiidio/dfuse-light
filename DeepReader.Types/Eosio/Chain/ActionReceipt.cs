using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action_receipt.hpp
/// </summary>
public sealed class ActionReceipt : PooledObject<ActionReceipt>, IEosioSerializable<ActionReceipt>, IFasterSerializable<ActionReceipt>
{
    public Name Receiver { get; set; }
    public Checksum256 ActionDigest { get; set; }
    public ulong GlobalSequence { get; set; }
    public ulong ReceiveSequence { get; set; }
    public TransactionTraceAuthSequence[] AuthSequence { get; set; }
    public uint CodeSequence { get; set; }
    public uint AbiSequence { get; set; }

    public ActionReceipt()
    {
        Receiver = Name.TypeEmpty;
        ActionDigest = Checksum256.TypeEmpty;
        AuthSequence = Array.Empty<TransactionTraceAuthSequence>();
    }

    public static ActionReceipt ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new ActionReceipt();

        obj.Receiver = Name.ReadFromBinaryReader(reader);
        obj.ActionDigest = Checksum256.ReadFromBinaryReader(reader);
        obj.GlobalSequence = reader.ReadUInt64();
        obj.ReceiveSequence = reader.ReadUInt64();

        obj.AuthSequence = new TransactionTraceAuthSequence[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.AuthSequence.Length; i++)
        {
            obj.AuthSequence[i] = TransactionTraceAuthSequence.ReadFromBinaryReader(reader);
        }

        obj.CodeSequence = (uint)reader.Read7BitEncodedInt();
        obj.AbiSequence = (uint)reader.Read7BitEncodedInt();

        return obj;
    }

    public static ActionReceipt ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new ActionReceipt();

        obj.Receiver = Name.ReadFromFaster(reader);
        obj.ActionDigest = Checksum256.ReadFromFaster(reader);
        obj.GlobalSequence = reader.ReadUInt64();
        obj.ReceiveSequence = reader.ReadUInt64();

        obj.AuthSequence = new TransactionTraceAuthSequence[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.AuthSequence.Length; i++)
        {
            obj.AuthSequence[i] = TransactionTraceAuthSequence.ReadFromFaster(reader);
        }

        obj.CodeSequence = (uint)reader.Read7BitEncodedInt();
        obj.AbiSequence = (uint)reader.Read7BitEncodedInt();

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        Receiver.WriteToFaster(writer);
        ActionDigest.WriteToFaster(writer);
        writer.Write(GlobalSequence);
        writer.Write(ReceiveSequence);

        writer.Write7BitEncodedInt(AuthSequence.Length);
        foreach (var transactionTraceAuthSequence in AuthSequence)
        {
            transactionTraceAuthSequence.WriteToFaster(writer);
        }

        writer.Write7BitEncodedInt((int)CodeSequence);
        writer.Write7BitEncodedInt((int)AbiSequence);
    }

    public new static void ReturnToPool(ActionReceipt obj)
    {
        Checksum256.ReturnToPool(obj.ActionDigest);
        foreach (var transactionTraceAuthSequence in obj.AuthSequence)
        {
            TransactionTraceAuthSequence.ReturnToPool(transactionTraceAuthSequence);
        }
        obj.AuthSequence = Array.Empty<TransactionTraceAuthSequence>();

        TypeObjectPool.Return(obj);
    }
}