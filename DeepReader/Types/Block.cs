namespace DeepReader.Types;

public class Block
{
	public string Id;//                               string                      `protobuf:"bytes,1,opt,name=id,proto3" json:"id,omitempty"`
	public uint Number;//                           uint32                      `protobuf:"varint,2,opt,name=number,proto3" json:"number,omitempty"`
	public uint Version;//                          uint32                      `protobuf:"varint,3,opt,name=version,proto3" json:"version,omitempty"`
	public BlockHeader Header;//                    *BlockHeader                `protobuf:"bytes,4,opt,name=header,proto3" json:"header,omitempty"`
	public string ProducerSignature;//                string                      `protobuf:"bytes,5,opt,name=producer_signature,json=producerSignature,proto3" json:"producer_signature,omitempty"`
	public Extension[] BlockExtensions;//                  []*Extension                `protobuf:"bytes,7,rep,name=block_extensions,json=blockExtensions,proto3" json:"block_extensions,omitempty"`
	public uint DposProposedIrreversibleBlocknum;// uint32                      `protobuf:"varint,8,opt,name=dpos_proposed_irreversible_blocknum,json=dposProposedIrreversibleBlocknum,proto3" json:"dpos_proposed_irreversible_blocknum,omitempty"`
	public uint DposIrreversibleBlocknum;//         uint32                      `protobuf:"varint,9,opt,name=dpos_irreversible_blocknum,json=dposIrreversibleBlocknum,proto3" json:"dpos_irreversible_blocknum,omitempty"`
	public BlockRootMerkle BlockrootMerkle;//                  *BlockRootMerkle            `protobuf:"bytes,11,opt,name=blockroot_merkle,json=blockrootMerkle,proto3" json:"blockroot_merkle,omitempty"`
	public ProducerToLastProduced[] ProducerToLastProduced;//           []*ProducerToLastProduced   `protobuf:"bytes,12,rep,name=producer_to_last_produced,json=producerToLastProduced,proto3" json:"producer_to_last_produced,omitempty"`
	public ProducerToLastImpliedIRB[] ProducerToLastImpliedIrb;//         []*ProducerToLastImpliedIRB `protobuf:"bytes,13,rep,name=producer_to_last_implied_irb,json=producerToLastImpliedIrb,proto3" json:"producer_to_last_implied_irb,omitempty"`
	public uint[] ConfirmCount;//                     []uint32                    `protobuf:"varint,15,rep,packed,name=confirm_count,json=confirmCount,proto3" json:"confirm_count,omitempty"`
	public PendingProducerSchedule PendingSchedule;//                  *PendingProducerSchedule    `protobuf:"bytes,16,opt,name=pending_schedule,json=pendingSchedule,proto3" json:"pending_schedule,omitempty"`
	public ActivatedProtocolFeatures ActivatedProtocolFeatures;//        *ActivatedProtocolFeatures  `protobuf:"bytes,17,opt,name=activated_protocol_features,json=activatedProtocolFeatures,proto3" json:"activated_protocol_features,omitempty"`
	public bool Validated;//                        bool                        `protobuf:"varint,18,opt,name=validated,proto3" json:"validated,omitempty"`
	public IList<RlimitOp> RlimitOps;//                        []*RlimitOp                 `protobuf:"bytes,19,rep,name=rlimit_ops,json=rlimitOps,proto3" json:"rlimit_ops,omitempty"`
	// The unfiltered transactions in this block when NO filtering has been applied,
	// (i.e. `filtering_applied = false`). When filtering has been applied on this block,
	// (i.e. `filtering_applied = true`), this field will be set to `nil` and instead, the
	// `filtered_transactions` will be populated with only filtered transactions.
	//
	// Use the helper getter method `Transactions()` to automatically pick the correct
	// field to use (`unfiltered_transactions` when `filtering_applied == false` and
	// `filtered_transactions` when `filtering_applied == true`).
	public IList<TransactionReceipt> UnfilteredTransactions;// []*TransactionReceipt `protobuf:"bytes,6,rep,name=unfiltered_transactions,json=unfilteredTransactions,proto3" json:"unfiltered_transactions,omitempty"`
	// The filtered transactions in this block when filtering has been applied,
	// (i.e. `filtering_applied = true`). This will be only the transactions
	// that matched the include filter CEL expression and did NOT match the exclude
	// filter CEL expression.
	//
	// Use the helper getter method `Transactions()` to automatically the correct
	// field (`unfiltered_transaction` when `filtering_applied == false` and
	// `filtered_transactions` when `filtering_applied == true`).
	public TransactionReceipt[] FilteredTransactions;// []*TransactionReceipt `protobuf:"bytes,47,rep,name=filtered_transactions,json=filteredTransactions,proto3" json:"filtered_transactions,omitempty"`
	// Number of transaction executed within this block when no filtering
	// is applied (`filtering_applied == false`).
	public uint UnfilteredTransactionCount;// uint32 `protobuf:"varint,22,opt,name=unfiltered_transaction_count,json=unfilteredTransactionCount,proto3" json:"unfiltered_transaction_count,omitempty"`
	// Number of transaction that were successfully executed within this block that are found in
	// the `filtered_transactions` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
	public uint FilteredTransactionCount;// uint32 `protobuf:"varint,48,opt,name=filtered_transaction_count,json=filteredTransactionCount,proto3" json:"filtered_transaction_count,omitempty"`
	// The unfiltered implicit transaction ops in this block when NO filtering has been applied,
	// (i.e. `filtering_applied = false`). When filtering has been applied on this block,
	// (i.e. `filtering_applied = true`), this field will be set to `nil` and instead, the
	// `filtered_implicit_transaction_ops` will be populated with only filtered implicit
	// transaction ops.
	//
	// Use the helper getter method `ImplicitTransactionOps()` to automatically pick the correct
	// field to use (`unfiltered_implicit_transaction_ops` when `filtering_applied == false` and
	// `filtered_implicit_transaction_ops` when `filtering_applied == true`).
	public IList<TrxOp> UnfilteredImplicitTransactionOps;// []*TrxOp `protobuf:"bytes,20,rep,name=unfiltered_implicit_transaction_ops,json=unfilteredImplicitTransactionOps,proto3" json:"unfiltered_implicit_transaction_ops,omitempty"`
	// The filtered implicit transaction ops in this block when filtering has been applied,
	// (i.e. `filtering_applied = true`). This will be only the implicit transaction ops
	// that matched the include filter CEL expression and did NOT match the exclude
	// filter CEL expression.
	//
	// Use the helper getter method `ImplicitTransactionOps()` to automatically the correct
	// field (`unfiltered_implicit_transaction_ops` when `filtering_applied == false` and
	// `filtered_implicit_transaction_ops` when `filtering_applied == true`).
	public TrxOp[] FilteredImplicitTransactionOps;// []*TrxOp `protobuf:"bytes,49,rep,name=filtered_implicit_transaction_ops,json=filteredImplicitTransactionOps,proto3" json:"filtered_implicit_transaction_ops,omitempty"`
	// The unfiltered transaction traces in this block when NO filtering has been applied,
	// (i.e. `filtering_applied = false`). When filtering has been applied on this block,
	// (i.e. `filtering_applied = true`), this field will be set to `nil` and instead, the
	// `filtered_transaction_traces` will be populated with only filtered transactions.
	//
	// Use the helper getter method `TransactionTraces()` to automatically pick the correct
	// field to use (`unfiltered_transaction_traces` when `filtering_applied == false` and
	// `filtered_transaction_traces` when `filtering_applied == true`).
	public IList<TransactionTrace> UnfilteredTransactionTraces;// []*TransactionTrace `protobuf:"bytes,21,rep,name=unfiltered_transaction_traces,json=unfilteredTransactionTraces,proto3" json:"unfiltered_transaction_traces,omitempty"`
	// The filtered transaction traces in this block when filtering has been applied,
	// (i.e. `filtering_applied = true`). This will be only the transaction trace
	// that matched the include filter CEL expression and did NOT match the exclude
	// filter CEL expression.
	//
	// Use the helper getter method `TransactionTraces()` to automatically pick the correct
	// field to use (`unfiltered_transaction_traces` when `filtering_applied == false` and
	// `filtered_transaction_traces` when `filtering_applied == true`).
	public TransactionTrace[] FilteredTransactionTraces;// []*TransactionTrace `protobuf:"bytes,46,rep,name=filtered_transaction_traces,json=filteredTransactionTraces,proto3" json:"filtered_transaction_traces,omitempty"`
	// Number of transaction trace executed within this block when no filtering
	// is applied (`filtering_applied == false`).
	public uint UnfilteredTransactionTraceCount;// uint32 `protobuf:"varint,23,opt,name=unfiltered_transaction_trace_count,json=unfilteredTransactionTraceCount,proto3" json:"unfiltered_transaction_trace_count,omitempty"`
	// Number of transaction trace that were successfully executed within this block that are found in
	// the `filtered_transaction_traces` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
	public uint FilteredTransactionTraceCount;// uint32 `protobuf:"varint,43,opt,name=filtered_transaction_trace_count,json=filteredTransactionTraceCount,proto3" json:"filtered_transaction_trace_count,omitempty"`
	// Number of top-level actions that were successfully executed within this block when no filtering
	// is applied (`filtering_applied == false`).
	public uint UnfilteredExecutedInputActionCount;// uint32 `protobuf:"varint,24,opt,name=unfiltered_executed_input_action_count,json=unfilteredExecutedInputActionCount,proto3" json:"unfiltered_executed_input_action_count,omitempty"`
	// Number of top-level actions that were successfully executed within this block that are found in
	// the `filtered_transaction_traces` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
	public uint FilteredExecutedInputActionCount;// uint32 `protobuf:"varint,44,opt,name=filtered_executed_input_action_count,json=filteredExecutedInputActionCount,proto3" json:"filtered_executed_input_action_count,omitempty"`
	// Number of actions that were successfully executed within this block when no filtering
	// is applied (`filtering_applied == false`).
	public uint UnfilteredExecutedTotalActionCount;// uint32 `protobuf:"varint,25,opt,name=unfiltered_executed_total_action_count,json=unfilteredExecutedTotalActionCount,proto3" json:"unfiltered_executed_total_action_count,omitempty"`
	// Number of actions that were successfully executed within this block that are found in
	// the `filtered_transaction_traces` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
	public uint FilteredExecutedTotalActionCount;// uint32 `protobuf:"varint,45,opt,name=filtered_executed_total_action_count,json=filteredExecutedTotalActionCount,proto3" json:"filtered_executed_total_action_count,omitempty"`
	// This was a single string element representing a public key (eos-go#ecc.PublicKey).
	// It has been replaced by `valid_block_signing_authority_v2`.
	public string BlockSigningKey;// string `protobuf:"bytes,14,opt,name=block_signing_key,json=blockSigningKey,proto3" json:"block_signing_key,omitempty"`
	// This was a list of `{name, publicKey}` elements, each block being signed by a single key,
	// the schedule was simply a list of pair, each pair being the producer name and it's public key
	// used to sign the block.
	public ProducerSchedule ActiveScheduleV1;// *ProducerSchedule `protobuf:"bytes,10,opt,name=active_schedule_v1,json=activeScheduleV1,proto3" json:"active_schedule_v1,omitempty"`
	// This replaces `block_signing_key` with a richer structure
	// able to handle the weighted threshold multisig for block producers.
	//
	// This can be downgraded to the old `block_signing_key` simply by taking
	// the first key present in the list. This is of course simple and not
	// accurate anymore in EOSIO 2.0 system where `WTMSIG_BLOCK_SIGNATURES`
	// has been activated AND block producers starts signing blocks with
	// more than one key.
	//
	// See BlockSigningAuthority for further details
	public BlockSigningAuthority ValidBlockSigningAuthorityV2;// *BlockSigningAuthority `protobuf:"bytes,30,opt,name=valid_block_signing_authority_v2,json=validBlockSigningAuthorityV2,proto3" json:"valid_block_signing_authority_v2,omitempty"`
	// This repleaces the old type `ProducerSchedule` for the `active_schedule`
	// field. This was only a type change in EOSIO 2.0, the field's name remained
	// the same.
	//
	// This is the new schedule data layout which is richer than it's oldest
	// counterpart. The inner element for a producer can then be composed with
	// multiple keys, each with their own weight and the threshold required to
	// accept the block signature.
	public ProducerAuthoritySchedule ActiveScheduleV2;// *ProducerAuthoritySchedule `protobuf:"bytes,31,opt,name=active_schedule_v2,json=activeScheduleV2,proto3" json:"active_schedule_v2,omitempty"`
	// Wheter or not a filtering process was run on this block. The filtering process sets to nil
	// the `unfiltered_transaction_traces` to `nil` and populate the `filtered_transaction_traces`
	// according to the `filtering_include_filter_expr` and `filtering_exclude_filter_expr` CEL
	// expressions. A transaction will be present in the `filtered_transaction_traces` array if
	// it matched the `filtering_include_filter_expr` and did *NOT* match the `filtering_exclude_filter_expr`.
	//
	// Moreover, each matching action that brought the transaction to be in `filtered_transaction_traces`
	// array will have a `filtering_matched` flag set on it to broadcast the fact that this action
	// match the inclusion/exclusion list.
	//
	// This flag controls all `filtered_*` and `unfiltered_*` elements on the Block structure and on
	// substructures if present.
	public bool FilteringApplied;// bool `protobuf:"varint,40,opt,name=filtering_applied,json=filteringApplied,proto3" json:"filtering_applied,omitempty"`
	// The CEL filter expression used to include transaction in `filtered_transaction_traces` array, works
	// in combination with `filtering_exclude_filter_expr` value.
	public string FilteringIncludeFilterExpr;// string `protobuf:"bytes,41,opt,name=filtering_include_filter_expr,json=filteringIncludeFilterExpr,proto3" json:"filtering_include_filter_expr,omitempty"`
	// The CEL filter expression used to exclude transaction in `filtered_transaction_traces` array, works
	// in combination with `filtering_include_filter_expr` value.
	public string FilteringExcludeFilterExpr;// string `protobuf:"bytes,42,opt,name=filtering_exclude_filter_expr,json=filteringExcludeFilterExpr,proto3" json:"filtering_exclude_filter_expr,omitempty"`
	// The CEL filter expression used to include system actions, required by some systems, works
	// in combination with the two other filters above.
	public string FilteringSystemActionsIncludeFilterExpr;// string   `protobuf:"bytes,50,opt,name=filtering_system_actions_include_filter_expr,json=filteringSystemActionsIncludeFilterExpr,proto3" json:"filtering_system_actions_include_filter_expr,omitempty"`
	//XXX_NoUnkeyedLiteral                    struct{} `json:"-"`
	//XXX_unrecognized                        []byte   `json:"-"`
	//XXX_sizecache                           int32    `json:"-"`

// func (m *Block) Reset()         { *m = Block{} }
// func (m *Block) String() string { return proto.CompactTextString(m) }
// func (*Block) ProtoMessage()    {}
// func (*Block) Descriptor() ([]byte, []int) {
// 	return fileDescriptor_3286b8d338e80dff, []int{0}
// }
//
// func (m *Block) XXX_Unmarshal(b []byte) error {
// 	return xxx_messageInfo_Block.Unmarshal(m, b)
// }
// func (m *Block) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
// 	return xxx_messageInfo_Block.Marshal(b, m, deterministic)
// }
// func (m *Block) XXX_Merge(src proto.Message) {
// 	xxx_messageInfo_Block.Merge(m, src)
// }
// func (m *Block) XXX_Size() int {
// 	return xxx_messageInfo_Block.Size(m)
// }
// func (m *Block) XXX_DiscardUnknown() {
// 	xxx_messageInfo_Block.DiscardUnknown(m)
// }
//
// var xxx_messageInfo_Block proto.InternalMessageInfo
//
// func (m *Block) GetId() string {
// 	if m != nil {
// 		return m.Id
// 	}
// 	return ""
// }
//
// func (m *Block) GetNumber() uint32 {
// 	if m != nil {
// 		return m.Number
// 	}
// 	return 0
// }
//
// func (m *Block) GetVersion() uint32 {
// 	if m != nil {
// 		return m.Version
// 	}
// 	return 0
// }
//
// func (m *Block) GetHeader() *BlockHeader {
// 	if m != nil {
// 		return m.Header
// 	}
// 	return nil
// }
//
// func (m *Block) GetProducerSignature() string {
// 	if m != nil {
// 		return m.ProducerSignature
// 	}
// 	return ""
// }
//
// func (m *Block) GetBlockExtensions() []*Extension {
// 	if m != nil {
// 		return m.BlockExtensions
// 	}
// 	return nil
// }
//
// func (m *Block) GetDposProposedIrreversibleBlocknum() uint32 {
// 	if m != nil {
// 		return m.DposProposedIrreversibleBlocknum
// 	}
// 	return 0
// }
//
// func (m *Block) GetDposIrreversibleBlocknum() uint32 {
// 	if m != nil {
// 		return m.DposIrreversibleBlocknum
// 	}
// 	return 0
// }
//
// func (m *Block) GetBlockrootMerkle() *BlockRootMerkle {
// 	if m != nil {
// 		return m.BlockrootMerkle
// 	}
// 	return nil
// }
//
// func (m *Block) GetProducerToLastProduced() []*ProducerToLastProduced {
// 	if m != nil {
// 		return m.ProducerToLastProduced
// 	}
// 	return nil
// }
//
// func (m *Block) GetProducerToLastImpliedIrb() []*ProducerToLastImpliedIRB {
// 	if m != nil {
// 		return m.ProducerToLastImpliedIrb
// 	}
// 	return nil
// }
//
// func (m *Block) GetConfirmCount() []uint32 {
// 	if m != nil {
// 		return m.ConfirmCount
// 	}
// 	return nil
// }
//
// func (m *Block) GetPendingSchedule() *PendingProducerSchedule {
// 	if m != nil {
// 		return m.PendingSchedule
// 	}
// 	return nil
// }
//
// func (m *Block) GetActivatedProtocolFeatures() *ActivatedProtocolFeatures {
// 	if m != nil {
// 		return m.ActivatedProtocolFeatures
// 	}
// 	return nil
// }
//
// func (m *Block) GetValidated() bool {
// 	if m != nil {
// 		return m.Validated
// 	}
// 	return false
// }
//
// func (m *Block) GetRlimitOps() []*RlimitOp {
// 	if m != nil {
// 		return m.RlimitOps
// 	}
// 	return nil
// }
//
// func (m *Block) GetUnfilteredTransactions() []*TransactionReceipt {
// 	if m != nil {
// 		return m.UnfilteredTransactions
// 	}
// 	return nil
// }
//
// func (m *Block) GetFilteredTransactions() []*TransactionReceipt {
// 	if m != nil {
// 		return m.FilteredTransactions
// 	}
// 	return nil
// }
//
// func (m *Block) GetUnfilteredTransactionCount() uint32 {
// 	if m != nil {
// 		return m.UnfilteredTransactionCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetFilteredTransactionCount() uint32 {
// 	if m != nil {
// 		return m.FilteredTransactionCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetUnfilteredImplicitTransactionOps() []*TrxOp {
// 	if m != nil {
// 		return m.UnfilteredImplicitTransactionOps
// 	}
// 	return nil
// }
//
// func (m *Block) GetFilteredImplicitTransactionOps() []*TrxOp {
// 	if m != nil {
// 		return m.FilteredImplicitTransactionOps
// 	}
// 	return nil
// }
//
// func (m *Block) GetUnfilteredTransactionTraces() []*TransactionTrace {
// 	if m != nil {
// 		return m.UnfilteredTransactionTraces
// 	}
// 	return nil
// }
//
// func (m *Block) GetFilteredTransactionTraces() []*TransactionTrace {
// 	if m != nil {
// 		return m.FilteredTransactionTraces
// 	}
// 	return nil
// }
//
// func (m *Block) GetUnfilteredTransactionTraceCount() uint32 {
// 	if m != nil {
// 		return m.UnfilteredTransactionTraceCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetFilteredTransactionTraceCount() uint32 {
// 	if m != nil {
// 		return m.FilteredTransactionTraceCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetUnfilteredExecutedInputActionCount() uint32 {
// 	if m != nil {
// 		return m.UnfilteredExecutedInputActionCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetFilteredExecutedInputActionCount() uint32 {
// 	if m != nil {
// 		return m.FilteredExecutedInputActionCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetUnfilteredExecutedTotalActionCount() uint32 {
// 	if m != nil {
// 		return m.UnfilteredExecutedTotalActionCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetFilteredExecutedTotalActionCount() uint32 {
// 	if m != nil {
// 		return m.FilteredExecutedTotalActionCount
// 	}
// 	return 0
// }
//
// func (m *Block) GetBlockSigningKey() string {
// 	if m != nil {
// 		return m.BlockSigningKey
// 	}
// 	return ""
// }
//
// func (m *Block) GetActiveScheduleV1() *ProducerSchedule {
// 	if m != nil {
// 		return m.ActiveScheduleV1
// 	}
// 	return nil
// }
//
// func (m *Block) GetValidBlockSigningAuthorityV2() *BlockSigningAuthority {
// 	if m != nil {
// 		return m.ValidBlockSigningAuthorityV2
// 	}
// 	return nil
// }
//
// func (m *Block) GetActiveScheduleV2() *ProducerAuthoritySchedule {
// 	if m != nil {
// 		return m.ActiveScheduleV2
// 	}
// 	return nil
// }
//
// func (m *Block) GetFilteringApplied() bool {
// 	if m != nil {
// 		return m.FilteringApplied
// 	}
// 	return false
// }
//
// func (m *Block) GetFilteringIncludeFilterExpr() string {
// 	if m != nil {
// 		return m.FilteringIncludeFilterExpr
// 	}
// 	return ""
// }
//
// func (m *Block) GetFilteringExcludeFilterExpr() string {
// 	if m != nil {
// 		return m.FilteringExcludeFilterExpr
// 	}
// 	return ""
// }
//
// func (m *Block) GetFilteringSystemActionsIncludeFilterExpr() string {
// 	if m != nil {
// 		return m.FilteringSystemActionsIncludeFilterExpr
// 	}
// 	return ""
// }
}