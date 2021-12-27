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
	public uint BlockTime = 0;//*timestamp.Timestamp
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
	public Exception? Exception = new Exception();// *Exception

	public ulong? ErrorCode = 0;//uint64

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


	// Index within block's unfiltered execution traces
	public ulong Index = 0;//uint64

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
	public Timestamp BlockTime = new Timestamp();//*timestamp.Timestamp
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

public class ActionTrace
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
	public Timestamp BlockTime = new Timestamp();//*timestamp.Timestamp
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