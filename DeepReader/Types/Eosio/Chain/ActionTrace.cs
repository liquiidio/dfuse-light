using DeepReader.EosTypes;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class ActionTrace {

    public VarUint32 ActionOrdinal = 0;

    public VarUint32 CreatorActionOrdinal = 0;

    public VarUint32 ClosestUnnotifiedAncestorActionOrdinal = 0;

    public ActionReceipt? Receipt;

    public Name Receiver = Name.Empty;

    public Action Act = new();//

    public bool ContextFree = false;

    public long ElapsedUs = 0;//

    public string Console = string.Empty;// eos.SafeString          `json:"console"`//

    public TransactionId TransactionId = string.Empty;

    public uint BlockNum = 0;

    public Timestamp BlockTime = 0;

    public BlockId? ProducerBlockID;

    public AccountDelta[] AccountRAMDeltas = Array.Empty<AccountDelta>();

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
}