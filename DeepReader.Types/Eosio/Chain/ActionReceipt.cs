using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action_receipt.hpp
/// </summary>
public class ActionReceipt : IEosioSerializable<ActionReceipt>
{
    public Name Receiver;
    public Checksum256 ActionDigest;
    public ulong GlobalSequence;
    public ulong ReceiveSequence;
    public TransactionTraceAuthSequence[] AuthSequence;
    public VarUint32 CodeSequence;
    public VarUint32 AbiSequence;

    public ActionReceipt(BinaryReader reader)
    {
        Receiver = reader.ReadName();
        ActionDigest = reader.ReadChecksum256();
        GlobalSequence = reader.ReadUInt64();
        ReceiveSequence = reader.ReadUInt64();

        AuthSequence = new TransactionTraceAuthSequence[reader.Read7BitEncodedInt()];
        CodeSequence = 0;
        AbiSequence = 0;
        for (int i = 0; i < AuthSequence.Length; i++)
        {
            AuthSequence[i] = TransactionTraceAuthSequence.ReadFromBinaryReader(reader);
        }

        CodeSequence = reader.ReadVarUint32Obj();
        AbiSequence = reader.ReadVarUint32Obj();
    }

    public static ActionReceipt ReadFromBinaryReader(BinaryReader reader)
    {
        return new ActionReceipt(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Receiver);
        writer.WriteChecksum256(ActionDigest);
        writer.Write(GlobalSequence);
        writer.Write(ReceiveSequence);

        writer.Write(AuthSequence.Length);
        foreach (var transactionTraceAuthSequence in AuthSequence)
        {
            transactionTraceAuthSequence.WriteToBinaryWriter(writer);
        }

        writer.Write(CodeSequence);
        writer.Write(AbiSequence);
    }
}