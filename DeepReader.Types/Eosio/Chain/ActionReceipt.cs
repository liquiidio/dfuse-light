using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action_receipt.hpp
/// </summary>
public sealed class ActionReceipt : PooledObject<ActionReceipt>, IEosioSerializable<ActionReceipt>
{
    public Name Receiver;
    public Checksum256 ActionDigest;
    public ulong GlobalSequence;
    public ulong ReceiveSequence;
    public TransactionTraceAuthSequence[] AuthSequence;
    public uint CodeSequence;
    public uint AbiSequence;

    public ActionReceipt()
    {
        Receiver = Name.TypeEmpty;
        ActionDigest = Checksum256.TypeEmpty;
        AuthSequence = Array.Empty<TransactionTraceAuthSequence>();
    }

    public static ActionReceipt ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
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

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        Receiver.WriteToBinaryWriter(writer);
        ActionDigest.WriteToBinaryWriter(writer);
        writer.Write(GlobalSequence);
        writer.Write(ReceiveSequence);

        writer.Write7BitEncodedInt(AuthSequence.Length);
        foreach (var transactionTraceAuthSequence in AuthSequence)
        {
            transactionTraceAuthSequence.WriteToBinaryWriter(writer);
        }

        writer.Write7BitEncodedInt((int)CodeSequence);
        writer.Write7BitEncodedInt((int)AbiSequence);

        TypeObjectPool.Return(this);
    }
}