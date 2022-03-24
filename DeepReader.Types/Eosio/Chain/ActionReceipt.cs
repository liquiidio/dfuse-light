using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action_receipt.hpp
/// </summary>
public class ActionReceipt : IEosioSerializable<ActionReceipt>
{
    public Name Receiver = string.Empty;
    public Checksum256 ActionDigest = string.Empty;
    public ulong GlobalSequence = 0;
    public ulong ReceiveSequence = 0;
    public TransactionTraceAuthSequence[] AuthSequence = Array.Empty<TransactionTraceAuthSequence>();
    public VarUint32 CodeSequence = 0;
    public VarUint32 AbiSequence = 0;

    public static ActionReceipt ReadFromBinaryReader(BinaryReader reader)
    {
        var actionReceipt = new ActionReceipt()
        {
            Receiver = reader.ReadName(),
            ActionDigest = reader.ReadChecksum256(),
            GlobalSequence = reader.ReadUInt64(),
            ReceiveSequence = reader.ReadUInt64()
        };

        actionReceipt.AuthSequence = new TransactionTraceAuthSequence[reader.Read7BitEncodedInt()];
        for (int i = 0; i < actionReceipt.AuthSequence.Length; i++)
        {
            actionReceipt.AuthSequence[i] = TransactionTraceAuthSequence.ReadFromBinaryReader(reader);
        }

        actionReceipt.CodeSequence = reader.ReadVarUint32Obj();
        actionReceipt.AbiSequence = reader.ReadVarUint32Obj();
        return actionReceipt;
    }
}