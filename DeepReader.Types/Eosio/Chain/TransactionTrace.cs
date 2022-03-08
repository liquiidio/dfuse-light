using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class TransactionTrace
{
	// SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
	public TransactionId Id = TransactionId.Empty;
	// Reference to the block number in which this transaction was executed.
	public uint BlockNum = 0;
	// Reference to the block time this transaction was executed in
	public Timestamp BlockTime = Timestamp.Zero;
	// Reference to the block ID this transaction was executed in
	public Checksum256? ProducerBlockId = Checksum256.Empty;
	// Receipt of execution of this transaction
	public TransactionReceiptHeader? Receipt;
	public long Elapsed = 0;
	public ulong NetUsage = 0;
	// Whether this transaction was taken from a scheduled transactions pool for
	// execution (delayed)
	public bool Scheduled = false;
	// Traces of each action within the transaction, including all notified and
	// nested actions.
	public ActionTrace[] ActionTraces = Array.Empty<ActionTrace>();

	// Account RAM Delta - ignored in dfuse
	public AccountRamDelta? AccountRamDelta;

	// Trace of a failed deferred transaction, if any.
	public TransactionTrace? FailedDtrxTrace;

	// Exception leading to the failed dtrx trace.
	public Except? Exception;

	public ulong? ErrorCode;

	// List of database operations this transaction entailed
	public IList<DbOp> DbOps { get; set; } = new List<DbOp>();//[]*DBOp
	// List of deferred transactions operations this transaction entailed
	public IList<DTrxOp> DtrxOps { get; set; } = new List<DTrxOp>();//[]*DTrxOp
	// List of feature switching operations (changes to feature switches in
	// nodeos) this transaction entailed
	public IList<FeatureOp> FeatureOps { get; set; } = new List<FeatureOp>();//[]*FeatureOp
	// List of permission changes operations
	public IList<PermOp> PermOps { get; set; } = new List<PermOp>();//[]*PermOp
	// List of RAM consumption/redemption
	public IList<RamOp> RamOps { get; set; } = new List<RamOp>();//[]*RAMOp
	// List of RAM correction operations (happens only once upon feature
	// activation)
	public IList<RamCorrectionOp> RamCorrectionOps { get; set; } = new List<RamCorrectionOp>();//[]*RAMCorrectionOp
	// List of changes to rate limiting values
	public IList<RlimitOp> RlimitOps { get; set; } = new List<RlimitOp>();//[]*RlimitOp
	// List of table creations/deletions
	public IList<TableOp> TableOps { get; set; } = new List<TableOp>();//[]*TableOp
	// Tree of creation, rather than execution
	public CreationFlatNode[] CreationTree { get; set; } = Array.Empty<CreationFlatNode>();//[]*CreationFlatNode

    // Index within block's unfiltered execution traces
	public ulong Index { get; set; } = 0;

}

public class Except {
	public long Code;
	public string Name = string.Empty;
	public string Message = string.Empty;
	public ExceptLogMessage[] Stack = Array.Empty<ExceptLogMessage>();
}

public class ExceptLogMessage {
	public ExceptLogContext Context = new();
	public string Format = string.Empty;
	public string Data = string.Empty;// json.RawMessage
}

public class ExceptLogContext {
	public byte Level;//ExceptLogLevel
	public string File = string.Empty;
	public ulong Line;
	public string Method = string.Empty;
	public string Hostname = string.Empty;
	public string ThreadName = string.Empty;
	public Timestamp Timestamp = Timestamp.Zero;//JSONTime
	public ExceptLogContext? Context;
}

public class CreationFlatNode
{
	public int WalkIndex = 0;
	public int CreatorActionIndex = 0;//int32
	public int ExecutionActionIndex = 0;//uint32
}