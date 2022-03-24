using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class ActionTrace : IEosioSerializable<ActionTrace>
{

    public VarUint32 ActionOrdinal = 0;

    public VarUint32 CreatorActionOrdinal = 0;

    public VarUint32 ClosestUnnotifiedAncestorActionOrdinal = 0;

    public ActionReceipt? Receipt;

    public Name Receiver = Name.Empty;

    public Action Act = new();

    public bool ContextFree = false;

    public long ElapsedUs = 0;

    public string Console = string.Empty;

    public TransactionId TransactionId = string.Empty;

    public uint BlockNum = 0;

    public Timestamp BlockTime = 0;

    public Checksum256? ProducerBlockId;

    public AccountDelta[] AccountRamDeltas = Array.Empty<AccountDelta>();

    // TODO Added in 2.1.x - this seems to be wrong with Mandel
    //	public AccountDelta[] AccountDiskDeltas = Array.Empty<AccountDelta>();

    public Except? Except;

    public ulong? ErrorCode;

    public char[] ReturnValue = Array.Empty<char>();	// TODO, string?

    public bool IsInput()
    {
        return GetCreatorActionOrdinal() == 0;
    }

    private uint GetCreatorActionOrdinal()
    {
        return CreatorActionOrdinal;
    }

    public static ActionTrace ReadFromBinaryReader(BinaryReader reader)
    {
        var actionTrace = new ActionTrace()
        {
            ActionOrdinal = reader.ReadVarUint32Obj(),
            CreatorActionOrdinal = reader.ReadVarUint32Obj(),
            ClosestUnnotifiedAncestorActionOrdinal = reader.ReadVarUint32Obj(),
        };

        var readActionReceipt = reader.ReadBoolean();

        if (readActionReceipt)
            actionTrace.Receipt = ActionReceipt.ReadFromBinaryReader(reader);

        actionTrace.Receiver = reader.ReadName();
        actionTrace.Act = Action.ReadFromBinaryReader(reader);
        actionTrace.ContextFree = reader.ReadBoolean();
        actionTrace.ElapsedUs = reader.ReadInt64();
        actionTrace.Console = reader.ReadString();
        actionTrace.TransactionId = reader.ReadTransactionId();
        actionTrace.BlockNum = reader.ReadUInt32();
        actionTrace.BlockTime = reader.ReadTimestamp();

        var readProducerBlockId = reader.ReadBoolean();

        if (readProducerBlockId)
            actionTrace.ProducerBlockId = reader.ReadChecksum256();

        actionTrace.AccountRamDeltas = new AccountDelta[reader.Read7BitEncodedInt()];
        for (int i = 0; i < actionTrace.AccountRamDeltas.Length; i++)
        {
            actionTrace.AccountRamDeltas[i] = AccountDelta.ReadFromBinaryReader(reader);
        }

        var readExcept = reader.ReadBoolean();

        if (readExcept)
            actionTrace.Except = Except.ReadFromBinaryReader(reader);

        var readErrorCode = reader.ReadBoolean();

        if (readErrorCode)
            actionTrace.ErrorCode = reader.ReadUInt64();

        actionTrace.ReturnValue = reader.ReadChars(reader.Read7BitEncodedInt());

        return actionTrace;
    }
}