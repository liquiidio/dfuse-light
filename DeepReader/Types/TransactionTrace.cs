namespace DeepReader.Types;

public class TransactionTrace
{
	// SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
	public string Id;// string `protobuf:"bytes,1,opt,name=id,proto3" json:"id,omitempty"`
	// Reference to the block number in which this transaction was executed.
	public ulong BlockNum;// uint64 `protobuf:"varint,2,opt,name=block_num,json=blockNum,proto3" json:"block_num,omitempty"`
	// Index within block's unfiltered execution traces
	public ulong Index;// uint64 `protobuf:"varint,26,opt,name=index,proto3" json:"index,omitempty"`
	// Reference to the block time this transaction was executed in
	public Timestamp BlockTime;// *timestamp.Timestamp `protobuf:"bytes,3,opt,name=block_time,json=blockTime,proto3" json:"block_time,omitempty"`
	// Reference to the block ID this transaction was executed in
	public string ProducerBlockId;// string `protobuf:"bytes,4,opt,name=producer_block_id,json=producerBlockId,proto3" json:"producer_block_id,omitempty"`
	// Receipt of execution of this transaction
	public TransactionReceiptHeader Receipt;//  *TransactionReceiptHeader `protobuf:"bytes,5,opt,name=receipt,proto3" json:"receipt,omitempty"`
	public ulong Elapsed;//  int64                     `protobuf:"varint,6,opt,name=elapsed,proto3" json:"elapsed,omitempty"`
	public ulong NetUsage;// uint64                    `protobuf:"varint,7,opt,name=net_usage,json=netUsage,proto3" json:"net_usage,omitempty"`
	// Whether this transaction was taken from a scheduled transactions pool for
	// execution (delayed)
	public bool Scheduled;// bool `protobuf:"varint,8,opt,name=scheduled,proto3" json:"scheduled,omitempty"`
	// Traces of each action within the transaction, including all notified and
	// nested actions.
	public ActionTrace[] ActionTraces;// []*ActionTrace `protobuf:"bytes,9,rep,name=action_traces,json=actionTraces,proto3" json:"action_traces,omitempty"`
	// Trace of a failed deferred transaction, if any.
	public TransactionTrace FailedDtrxTrace;// *TransactionTrace `protobuf:"bytes,10,opt,name=failed_dtrx_trace,json=failedDtrxTrace,proto3" json:"failed_dtrx_trace,omitempty"`
	// Exception leading to the failed dtrx trace.
	public Exception Exception;// *Exception `protobuf:"bytes,15,opt,name=exception,proto3" json:"exception,omitempty"`
	public ulong ErrorCode;// uint64     `protobuf:"varint,16,opt,name=error_code,json=errorCode,proto3" json:"error_code,omitempty"`
	// List of database operations this transaction entailed
	public ICollection<DBOp> DbOps;// []*DBOp `protobuf:"bytes,17,rep,name=db_ops,json=dbOps,proto3" json:"db_ops,omitempty"`
	// List of deferred transactions operations this transaction entailed
	public ICollection<DTrxOp> DtrxOps;// []*DTrxOp `protobuf:"bytes,18,rep,name=dtrx_ops,json=dtrxOps,proto3" json:"dtrx_ops,omitempty"`
	// List of feature switching operations (changes to feature switches in
	// nodeos) this transaction entailed
	public ICollection<FeatureOp> FeatureOps;// []*FeatureOp `protobuf:"bytes,19,rep,name=feature_ops,json=featureOps,proto3" json:"feature_ops,omitempty"`
	// List of permission changes operations
	public ICollection<PermOp> PermOps;// []*PermOp `protobuf:"bytes,20,rep,name=perm_ops,json=permOps,proto3" json:"perm_ops,omitempty"`
	// List of RAM consumption/redemption
	public ICollection<RAMOp> RamOps;// []*RAMOp `protobuf:"bytes,21,rep,name=ram_ops,json=ramOps,proto3" json:"ram_ops,omitempty"`
	// List of RAM correction operations (happens only once upon feature
	// activation)
	public ICollection<RAMCorrectionOp> RamCorrectionOps;// []*RAMCorrectionOp `protobuf:"bytes,22,rep,name=ram_correction_ops,json=ramCorrectionOps,proto3" json:"ram_correction_ops,omitempty"`
	// List of changes to rate limiting values
	public ICollection<RlimitOp> RlimitOps;// []*RlimitOp `protobuf:"bytes,23,rep,name=rlimit_ops,json=rlimitOps,proto3" json:"rlimit_ops,omitempty"`
	// List of table creations/deletions
	public ICollection<TableOp> TableOps;// []*TableOp `protobuf:"bytes,24,rep,name=table_ops,json=tableOps,proto3" json:"table_ops,omitempty"`
	// Tree of creation, rather than execution
	public CreationFlatNode[] CreationTree;//         []*CreationFlatNode `protobuf:"bytes,25,rep,name=creation_tree,json=creationTree,proto3" json:"creation_tree,omitempty"`
	//XXX_NoUnkeyedLiteral struct{}            `json:"-"`
	//XXX_unrecognized     []byte              `json:"-"`
	//XXX_sizecache        int32               `json:"-"`
}

public class CreationFlatNode
{
	public int WalkIndex;
	public int CreatorActionIndex;//   int32    `protobuf:"varint,1,opt,name=creator_action_index,json=creatorActionIndex,proto3" json:"creator_action_index,omitempty"`
	public int ExecutionActionIndex;// uint32   `protobuf:"varint,2,opt,name=execution_action_index,json=executionActionIndex,proto3" json:"execution_action_index,omitempty"`
	//XXX_NoUnkeyedLiteral struct{} `json:"-"`
	//XXX_unrecognized     []byte   `json:"-"`
	//XXX_sizecache        int32    `json:"-"`
}

public class ActionTrace
{
	public string Receiver;//                               string               `protobuf:"bytes,11,opt,name=receiver,proto3" json:"receiver,omitempty"`
	public ActionReceipt Receipt;//                                *ActionReceipt       `protobuf:"bytes,1,opt,name=receipt,proto3" json:"receipt,omitempty"`
	public Action Action;//                                 *Action              `protobuf:"bytes,2,opt,name=action,proto3" json:"action,omitempty"`
	public bool ContextFree;//                            bool                 `protobuf:"varint,3,opt,name=context_free,json=contextFree,proto3" json:"context_free,omitempty"`
	public long Elapsed;//                                int64                `protobuf:"varint,4,opt,name=elapsed,proto3" json:"elapsed,omitempty"`
	public string Console;//                                string               `protobuf:"bytes,5,opt,name=console,proto3" json:"console,omitempty"`
	public string TransactionId;//                          string               `protobuf:"bytes,6,opt,name=transaction_id,json=transactionId,proto3" json:"transaction_id,omitempty"`
	public ulong BlockNum;//                               uint64               `protobuf:"varint,7,opt,name=block_num,json=blockNum,proto3" json:"block_num,omitempty"`
	public string ProducerBlockId;//                        string               `protobuf:"bytes,8,opt,name=producer_block_id,json=producerBlockId,proto3" json:"producer_block_id,omitempty"`
	public Timestamp BlockTime;//                              *timestamp.Timestamp `protobuf:"bytes,9,opt,name=block_time,json=blockTime,proto3" json:"block_time,omitempty"`
	public AccountRAMDelta[] AccountRamDeltas;//                       []*AccountRAMDelta   `protobuf:"bytes,10,rep,name=account_ram_deltas,json=accountRamDeltas,proto3" json:"account_ram_deltas,omitempty"`
	public Exception Exception;//                              *Exception           `protobuf:"bytes,15,opt,name=exception,proto3" json:"exception,omitempty"`
	public ulong ErrorCode;//                              uint64               `protobuf:"varint,20,opt,name=error_code,json=errorCode,proto3" json:"error_code,omitempty"`
	public uint ActionOrdinal;//                          uint32               `protobuf:"varint,16,opt,name=action_ordinal,json=actionOrdinal,proto3" json:"action_ordinal,omitempty"`
	public uint CreatorActionOrdinal;//                   uint32               `protobuf:"varint,17,opt,name=creator_action_ordinal,json=creatorActionOrdinal,proto3" json:"creator_action_ordinal,omitempty"`
	public uint ClosestUnnotifiedAncestorActionOrdinal;// uint32               `protobuf:"varint,18,opt,name=closest_unnotified_ancestor_action_ordinal,json=closestUnnotifiedAncestorActionOrdinal,proto3" json:"closest_unnotified_ancestor_action_ordinal,omitempty"`
	public uint ExecutionIndex;//                         uint32               `protobuf:"varint,19,opt,name=execution_index,json=executionIndex,proto3" json:"execution_index,omitempty"`
	// Whether this action trace was a successful match, present only when filtering was applied on block. This
	// will be `true` if the Block `filtering_applied` is `true`, if the include CEL filter matched and
	// if the exclude CEL filter did NOT match.
	public bool FilteringMatched;// bool `protobuf:"varint,30,opt,name=filtering_matched,json=filteringMatched,proto3" json:"filtering_matched,omitempty"`
	// Whether this action trace was a successful system match, present only when filtering was applied on block.
	// This will be `true` if the Block `filtering_applied` is `true`, if the system actions include CEL filter
	// matched, supersedes any exclude CEL filter.
	public bool FilteringMatchedSystemActionFilter;// bool     `protobuf:"varint,31,opt,name=filtering_matched_system_action_filter,json=filteringMatchedSystemActionFilter,proto3" json:"filtering_matched_system_action_filter,omitempty"`
	//XXX_NoUnkeyedLiteral               struct{} `json:"-"`
	//XXX_unrecognized                   []byte   `json:"-"`
	//XXX_sizecache                      int32    `json:"-"`

	public bool IsInput()
	{
		return GetCreatorActionOrdinal() == 0;
	}

	private uint GetCreatorActionOrdinal()
	{
		return CreatorActionOrdinal;
	}
}