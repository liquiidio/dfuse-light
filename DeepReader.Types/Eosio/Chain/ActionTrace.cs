using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class ActionTrace : IEosioSerializable<ActionTrace>
{
    public VarUint32 ActionOrdinal;

    public VarUint32 CreatorActionOrdinal;

    public VarUint32 ClosestUnnotifiedAncestorActionOrdinal;

    public ActionReceipt? Receipt;

    public Name Receiver;

    public Action Act;

    public bool ContextFree;

    public long ElapsedUs;

    public string Console;

    public TransactionId TransactionId;

    public uint BlockNum;

    public Timestamp BlockTime;

    public Checksum256? ProducerBlockId;

    public AccountDelta[] AccountRamDeltas;

    // TODO Added in 2.1.x - this seems to be wrong with Mandel
    //	public AccountDelta[] AccountDiskDeltas = Array.Empty<AccountDelta>();

    public Except? Except;

    public ulong? ErrorCode;

    public char[] ReturnValue;	// TODO, string?

    public bool IsInput()
    {
        return GetCreatorActionOrdinal() == 0;
    }

    private uint GetCreatorActionOrdinal()
    {
        return CreatorActionOrdinal;
    }

    public ActionTrace(BinaryReader reader)
    {
        ActionOrdinal = reader.ReadVarUint32Obj();
        CreatorActionOrdinal = reader.ReadVarUint32Obj();
        ClosestUnnotifiedAncestorActionOrdinal = reader.ReadVarUint32Obj();

        var readActionReceipt = reader.ReadBoolean();

        if (readActionReceipt)
            Receipt = ActionReceipt.ReadFromBinaryReader(reader);

        Receiver = reader.ReadName();
        Act = Action.ReadFromBinaryReader(reader);
        ContextFree = reader.ReadBoolean();
        ElapsedUs = reader.ReadInt64();
        Console = reader.ReadString();
        TransactionId = reader.ReadTransactionId();
        BlockNum = reader.ReadUInt32();
        BlockTime = reader.ReadTimestamp();

        var readProducerBlockId = reader.ReadBoolean();

        if (readProducerBlockId)
            ProducerBlockId = reader.ReadChecksum256();

        AccountRamDeltas = new AccountDelta[reader.Read7BitEncodedInt()];
        for (int i = 0; i < AccountRamDeltas.Length; i++)
        {
            AccountRamDeltas[i] = AccountDelta.ReadFromBinaryReader(reader);
        }

        var readExcept = reader.ReadBoolean();

        if (readExcept)
            Except = Except.ReadFromBinaryReader(reader);

        var readErrorCode = reader.ReadBoolean();

        if (readErrorCode)
            ErrorCode = reader.ReadUInt64();

        ReturnValue = reader.ReadChars(reader.Read7BitEncodedInt());
    }

    public static ActionTrace ReadFromBinaryReader(BinaryReader reader)
    {
        return new ActionTrace(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(ActionOrdinal);
        writer.Write(CreatorActionOrdinal);
        writer.Write(ClosestUnnotifiedAncestorActionOrdinal);

        if (Receipt != null)
        {
            writer.Write(true);
            Receipt.WriteToBinaryWriter(writer);
        }
        else
            writer.Write(false);

        writer.WriteName(Receiver);

        Act.WriteToBinaryWriter(writer);

        writer.Write(ContextFree);
        writer.Write(ElapsedUs);
        writer.Write(Console);

        TransactionId.WriteToBinaryWriter(writer);

        writer.Write(BlockNum);
        writer.Write(BlockTime.Ticks);

        if (ProducerBlockId != null)
        {
            writer.Write(true);
            writer.WriteChecksum256(ProducerBlockId);
        }
        else
            writer.Write(false);

        writer.Write(AccountRamDeltas.Length);
        foreach (var accountRamDelta in AccountRamDeltas)
        {
            accountRamDelta.WriteToBinaryWriter(writer);
        }

        if (Except != null)
        {
            writer.Write(true);
            Except.WriteToBinaryWriter(writer);
        }
        else
            writer.Write(false);


        if (ErrorCode != null)
        {
            writer.Write(true);
            writer.Write(ErrorCode.Value);
        }
        else
            writer.Write(false);

        writer.Write(ReturnValue.Length);
        writer.Write(ReturnValue);
    }
}