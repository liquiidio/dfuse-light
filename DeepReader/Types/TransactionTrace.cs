using DeepReader.EosTypes;

namespace DeepReader.Types;

//transaction_id_type id;
//uint32_t block_num = 0;
//block_timestamp_type block_time;
//std::optional<block_id_type> producer_block_id;
//std::optional<transaction_receipt_header> receipt;
//fc::microseconds elapsed;
//uint64_t net_usage = 0;
//bool scheduled = false;
//vector<action_trace> action_traces;
//std::optional<account_delta> account_ram_delta;

//transaction_trace_ptr failed_dtrx_trace;
//std::optional<fc::exception> except;
//std::optional<uint64_t> error_code;
//std::exception_ptr except_ptr;

public class TransactionTrace
{
	// SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
	public Checksum256 Id = string.Empty;//string
	// Reference to the block number in which this transaction was executed.
	public uint BlockNum = 0;//uint64
	// Reference to the block time this transaction was executed in
	public Timestamp BlockTime = 0;//*timestamp.Timestamp
	// Reference to the block ID this transaction was executed in
	public Checksum256? ProducerBlockId = string.Empty;//string
	// Receipt of execution of this transaction
	public TransactionReceiptHeader? Receipt;//*TransactionReceiptHeader
	public long Elapsed = 0;//int64
	public ulong NetUsage = 0;//uint64
	// Whether this transaction was taken from a scheduled transactions pool for
	// execution (delayed)
	public bool Scheduled = false;//bool
	// Traces of each action within the transaction, including all notified and
	// nested actions.
	public ActionTrace[] ActionTraces = Array.Empty<ActionTrace>();//[]*ActionTrace

	// Account RAM Delta - ignored in dfuse
	public AccountRAMDelta? AccountRAMDelta;

	// Trace of a failed deferred transaction, if any.
	public TransactionTrace? FailedDtrxTrace;//*TransactionTrace

	// Exception leading to the failed dtrx trace.
	public Except? Exception;// *Exception

	public ulong? ErrorCode;//uint64

	// List of database operations this transaction entailed
	public IList<DBOp> DbOps { get; set; } = new List<DBOp>();//[]*DBOp
	// List of deferred transactions operations this transaction entailed
	public IList<DTrxOp> DtrxOps { get; set; } = new List<DTrxOp>();//[]*DTrxOp
	// List of feature switching operations (changes to feature switches in
	// nodeos) this transaction entailed
	public IList<FeatureOp> FeatureOps { get; set; } = new List<FeatureOp>();//[]*FeatureOp
	// List of permission changes operations
	public IList<PermOp> PermOps { get; set; } = new List<PermOp>();//[]*PermOp
	// List of RAM consumption/redemption
	public IList<RAMOp> RamOps { get; set; } = new List<RAMOp>();//[]*RAMOp
	// List of RAM correction operations (happens only once upon feature
	// activation)
	public IList<RAMCorrectionOp> RamCorrectionOps { get; set; } = new List<RAMCorrectionOp>();//[]*RAMCorrectionOp
	// List of changes to rate limiting values
	public IList<RlimitOp> RlimitOps { get; set; } = new List<RlimitOp>();//[]*RlimitOp
	// List of table creations/deletions
	public IList<TableOp> TableOps { get; set; } = new List<TableOp>();//[]*TableOp
	// Tree of creation, rather than execution
	public CreationFlatNode[] CreationTree { get; set; } = Array.Empty<CreationFlatNode>();//[]*CreationFlatNode


	// Index within block's unfiltered execution traces
	public ulong Index = 0;//uint64

}

public class Except {
	public long Code;
	public string Name;
	public string Message;
	public IList<ExceptLogMessage> Stack;
}

public class ExceptLogMessage {
	public ExceptLogContext Context;
	public string Format;
	public string Data;// json.RawMessage
}

public class ExceptLogContext {
	public byte Level;//ExceptLogLevel
	public string File;
	public ulong Line;
	public string Method;
	public string Hostname;
	public string ThreadName;
	public Timestamp Timestamp;//JSONTime
	public ExceptLogContext? Context;
}

public class TransactionTracee
{
	// SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
	public Checksum256 Id = string.Empty;//string
	// Reference to the block number in which this transaction was executed.
	public ulong BlockNum = 0;//uint64
	// Index within block's unfiltered execution traces
	public ulong Index = 0;//uint64
	// Reference to the block time this transaction was executed in
	public Timestamp BlockTime = 0;//*timestamp.Timestamp
	// Reference to the block ID this transaction was executed in
	public string ProducerBlockId = string.Empty;//string
	// Receipt of execution of this transaction
	public TransactionReceiptHeader Receipt = new TransactionReceiptHeader();//*TransactionReceiptHeader
	public ulong Elapsed = 0;//int64
	public ulong NetUsage = 0;//uint64
	// Whether this transaction was taken from a scheduled transactions pool for
	// execution (delayed)
	public bool Scheduled = false;//bool
	// Traces of each action within the transaction, including all notified and
	// nested actions.
	public ActionTrace[] ActionTraces = Array.Empty<ActionTrace>();//[]*ActionTrace
																   // Trace of a failed deferred transaction, if any.
	public TransactionTrace? FailedDtrxTrace;//*TransactionTrace
	// Exception leading to the failed dtrx trace.
	public Exception Exception = new Exception();// *Exception
	public ulong ErrorCode = 0;//uint64
	// List of database operations this transaction entailed
	public IList<DBOp> DbOps = new List<DBOp>();//[]*DBOp
	// List of deferred transactions operations this transaction entailed
	public IList<DTrxOp> DtrxOps = new List<DTrxOp>();//[]*DTrxOp
	// List of feature switching operations (changes to feature switches in
	// nodeos) this transaction entailed
	public IList<FeatureOp> FeatureOps = new List<FeatureOp>();//[]*FeatureOp
	// List of permission changes operations
	public IList<PermOp> PermOps = new List<PermOp>();//[]*PermOp
	// List of RAM consumption/redemption
	public IList<RAMOp> RamOps = new List<RAMOp>();//[]*RAMOp
	// List of RAM correction operations (happens only once upon feature
	// activation)
	public IList<RAMCorrectionOp> RamCorrectionOps = new List<RAMCorrectionOp>();//[]*RAMCorrectionOp
	// List of changes to rate limiting values
	public IList<RlimitOp> RlimitOps = new List<RlimitOp>();//[]*RlimitOp
	// List of table creations/deletions
	public IList<TableOp> TableOps = new List<TableOp>();//[]*TableOp
	// Tree of creation, rather than execution
	public CreationFlatNode[] CreationTree = Array.Empty<CreationFlatNode>();//[]*CreationFlatNode
}

public class CreationFlatNode
{
	public int WalkIndex = 0;
	public int CreatorActionIndex = 0;//int32
	public int ExecutionActionIndex = 0;//uint32
}

public class  ActionTrace {
	public VarUint32 ActionOrdinal = 0;

	public VarUint32 CreatorActionOrdinal = 0;

	public VarUint32 ClosestUnnotifiedAncestorActionOrdinal = 0;

	public ActionTraceReceipt? Receipt;

	public Name Receiver = string.Empty;

	public Action Action = new Action();

	public bool ContextFree = false;

	public long ElapsedUs = 0;

	public string Console = string.Empty;// eos.SafeString          `json:"console"`

	public Checksum256 TransactionID = string.Empty;

	public uint BlockNum = 0;

	public Timestamp BlockTime = 0;

	public Checksum256? ProducerBlockID;

	public AccountDelta[] AccountRAMDeltas = Array.Empty<AccountDelta>();

	// Added in 2.1.x
	public AccountDelta[] AccountDiskDeltas = Array.Empty<AccountDelta>();

	public Except? Except;

	public ulong? ErrorCode;
		// Added in 2.1.x
	public string ReturnValue = string.Empty;// eos.HexBytes `json:"return_value"`

	public bool IsInput()
	{
		return GetCreatorActionOrdinal() == 0;
	}

	private uint GetCreatorActionOrdinal()
	{
		return CreatorActionOrdinal;
	}
}

public class AccountDelta {
	public Name Account = string.Empty;
	public long Delta = 0;
}

public class ActionTraceReceipt {
	public Name Receiver = string.Empty;//                    `json:"receiver"`
	public Checksum256 ActionDigest = string.Empty;//                    `json:"act_digest"`
	public ulong GlobalSequence = 0;// Uint64                         `json:"global_sequence"`
	public ulong ReceiveSequence = 0;// Uint64                         `json:"recv_sequence"`
	public TransactionTraceAuthSequence[] AuthSequence = Array.Empty<TransactionTraceAuthSequence>();// []  `json:"auth_sequence"` // [["account", sequence], ["account", sequence]]
	public VarUint32 CodeSequence = 0;// Varuint32                      `json:"code_sequence"`
	public VarUint32 ABISequence = 0;// Varuint32                      `json:"abi_sequence"`
}

public class TransactionTraceAuthSequence {
	public Name Account = string.Empty;
	public ulong Sequence = 0;
}

public class ActionTracee
{
	public string Receiver = string.Empty;//string
	public ActionReceipt Receipt = new ActionReceipt();//*ActionReceipt
	public Action Action = new Action();//*Action
	public bool ContextFree = false;//bool
	public long Elapsed = 0;//int64
	public string Console = string.Empty;//string
	public string TransactionId = string.Empty;//string
	public ulong BlockNum = 0;//uint64
	public string ProducerBlockId = string.Empty;//string
	public Timestamp BlockTime = 0;//*timestamp.Timestamp
	public AccountRAMDelta[] AccountRamDeltas = Array.Empty<AccountRAMDelta>();//[]*AccountRAMDelta
	public Exception Exception = new Exception();//*Exception
	public ulong ErrorCode = 0;//uint64
	public uint ActionOrdinal = 0;//uint32
	public uint CreatorActionOrdinal = 0;//uint32
	public uint ClosestUnnotifiedAncestorActionOrdinal = 0;//uint32
	public uint ExecutionIndex = 0;//uint32
	// Whether this action trace was a successful match, present only when filtering was applied on block. This
	// will be `true` if the Block `filtering_applied` is `true`, if the include CEL filter matched and
	// if the exclude CEL filter did NOT match.
	public bool FilteringMatched = false;//bool
	// Whether this action trace was a successful system match, present only when filtering was applied on block.
	// This will be `true` if the Block `filtering_applied` is `true`, if the system actions include CEL filter
	// matched, supersedes any exclude CEL filter.
	public bool FilteringMatchedSystemActionFilter = false;//bool

	public bool IsInput()
	{
		return GetCreatorActionOrdinal() == 0;
	}

	private uint GetCreatorActionOrdinal()
	{
		return CreatorActionOrdinal;
	}
}