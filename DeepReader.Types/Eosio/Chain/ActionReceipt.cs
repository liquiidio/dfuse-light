using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action_receipt.hpp
/// </summary>
public class ActionReceipt : IEosioSerializable<ActionReceipt>
{
    public Name Receiver { get; set; }
    public Checksum256 ActionDigest { get; set; }
    public ulong GlobalSequence { get; set; }
    public ulong ReceiveSequence { get; set; }
    public TransactionTraceAuthSequence[] AuthSequence { get; set; }
    public uint CodeSequence { get; set; }
    public uint AbiSequence { get; set; }

    public ActionReceipt(BinaryReader reader)
    {
        Receiver = reader.ReadName();
        ActionDigest = reader.ReadChecksum256();
        GlobalSequence = reader.ReadUInt64();
        ReceiveSequence = reader.ReadUInt64();

        AuthSequence = new TransactionTraceAuthSequence[reader.Read7BitEncodedInt()];
        for (int i = 0; i < AuthSequence.Length; i++)
        {
            AuthSequence[i] = TransactionTraceAuthSequence.ReadFromBinaryReader(reader);
        }

        CodeSequence = (uint)reader.Read7BitEncodedInt();
        AbiSequence = (uint)reader.Read7BitEncodedInt();
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

        writer.Write7BitEncodedInt(AuthSequence.Length);
        foreach (var transactionTraceAuthSequence in AuthSequence)
        {
            transactionTraceAuthSequence.WriteToBinaryWriter(writer);
        }

        writer.Write7BitEncodedInt((int)CodeSequence);
        writer.Write7BitEncodedInt((int)AbiSequence);
    }
}