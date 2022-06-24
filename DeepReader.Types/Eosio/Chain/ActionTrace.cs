using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public sealed class ActionTrace : IEosioSerializable<ActionTrace>, IFasterSerializable<ActionTrace>
{
    public uint ActionOrdinal;

    public uint CreatorActionOrdinal;

    public uint ClosestUnnotifiedAncestorActionOrdinal;

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

    public ActionTrace() { }

    public ActionTrace(BinaryBufferReader reader)
    {
        ActionOrdinal = (uint)reader.Read7BitEncodedInt();
        CreatorActionOrdinal = (uint)reader.Read7BitEncodedInt();
        ClosestUnnotifiedAncestorActionOrdinal = (uint)reader.Read7BitEncodedInt();

        var readActionReceipt = reader.ReadBoolean();
        if (readActionReceipt)
            Receipt = ActionReceipt.ReadFromBinaryReader(reader);

        Receiver = Name.ReadFromBinaryReader(reader);
        Act = Action.ReadFromBinaryReader(reader);
        ContextFree = reader.ReadBoolean();
        ElapsedUs = reader.ReadInt64();
        Console = reader.ReadString();
        TransactionId = TransactionId.ReadFromBinaryReader(reader);
        BlockNum = reader.ReadUInt32();
        BlockTime = Timestamp.ReadFromBinaryReader(reader);

        var readProducerBlockId = reader.ReadBoolean();

        if (readProducerBlockId)
            ProducerBlockId = Checksum256.ReadFromBinaryReader(reader);

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

    public static ActionTrace ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new ActionTrace(reader);
    }

    public static ActionTrace ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = new ActionTrace()
        {
            ActionOrdinal = (uint)reader.Read7BitEncodedInt(),
            CreatorActionOrdinal = (uint)reader.Read7BitEncodedInt(),
            ClosestUnnotifiedAncestorActionOrdinal = (uint)reader.Read7BitEncodedInt()
        };

        var readActionReceipt = reader.ReadBoolean();
        if (readActionReceipt)
            obj.Receipt = ActionReceipt.ReadFromFaster(reader);

        obj.Receiver = Name.ReadFromFaster(reader);
        obj.Act = Action.ReadFromFaster(reader);
        obj.ContextFree = reader.ReadBoolean();
        obj.ElapsedUs = reader.ReadInt64();
        obj.Console = reader.ReadString();
        obj.TransactionId = TransactionId.ReadFromFaster(reader);
        obj.BlockNum = reader.ReadUInt32();
        obj.BlockTime = Timestamp.ReadFromFaster(reader);

        var readProducerBlockId = reader.ReadBoolean();

        if (readProducerBlockId)
            obj.ProducerBlockId = Checksum256.ReadFromFaster(reader);

        obj.AccountRamDeltas = new AccountDelta[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.AccountRamDeltas.Length; i++)
        {
            obj.AccountRamDeltas[i] = AccountDelta.ReadFromFaster(reader);
        }

        var readExcept = reader.ReadBoolean();
        if (readExcept)
            obj.Except = Except.ReadFromFaster(reader);

        var readErrorCode = reader.ReadBoolean();
        if (readErrorCode)
            obj.ErrorCode = reader.ReadUInt64();

        obj.ReturnValue = reader.ReadChars(reader.Read7BitEncodedInt());

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        writer.Write(ActionOrdinal);
        writer.Write(CreatorActionOrdinal);
        writer.Write(ClosestUnnotifiedAncestorActionOrdinal);

        if (Receipt != null)
        {
            writer.Write(true);
            Receipt.WriteToFaster(writer);
        }
        else
            writer.Write(false);

        Receiver.WriteToFaster(writer);

        Act.WriteToFaster(writer);

        writer.Write(ContextFree);
        writer.Write(ElapsedUs);
        writer.Write(Console);

        TransactionId.WriteToFaster(writer);

        writer.Write(BlockNum);
        BlockTime.WriteToFaster(writer);

        if (ProducerBlockId != null)
        {
            writer.Write(true);
            ProducerBlockId.WriteToFaster(writer);
        }
        else
            writer.Write(false);

        writer.Write(AccountRamDeltas.Length);
        foreach (var accountRamDelta in AccountRamDeltas)
        {
            accountRamDelta.WriteToFaster(writer);
        }

        if (Except != null)
        {
            writer.Write(true);
            Except.WriteToFaster(writer);
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