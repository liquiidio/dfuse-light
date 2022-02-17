using System.Text.Json;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public class Block
{
	public Checksum256 Id = Checksum256.Empty;
	public uint Number = 0;
	public uint Version = 0;
	public SignedBlockHeader Header = new();
	public string ProducerSignature = string.Empty;
	public Extension[] BlockExtensions = Array.Empty<Extension>();
	public uint DposProposedIrreversibleBlocknum = 0;
	public uint DposIrreversibleBlocknum = 0;
	public IncrementalMerkle BlockrootMerkle = new();
	public PairAccountNameBlockNum[] ProducerToLastProduced = Array.Empty<PairAccountNameBlockNum>();
	public PairAccountNameBlockNum[] ProducerToLastImpliedIrb = Array.Empty<PairAccountNameBlockNum>();
	public uint[] ConfirmCount = Array.Empty<uint>();
    public ScheduleInfo PendingSchedule = new();
	public ProtocolFeatureActivationSet? ActivatedProtocolFeatures;
	public bool Validated = false;
	public IList<RlimitOp> RlimitOps = new List<RlimitOp>();
	// The unfiltered transactions in this block when NO filtering has been applied,
	// (i.e. `filtering_applied = false`). When filtering has been applied on this block,
	// (i.e. `filtering_applied = true`), this field will be set to `nil` and instead, the
	// `filtered_transactions` will be populated with only filtered transactions.
	//
	// Use the helper getter method `Transactions()` to automatically pick the correct
	// field to use (`unfiltered_transactions` when `filtering_applied == false` and
	// `filtered_transactions` when `filtering_applied == true`).
	public IList<TransactionReceipt> UnfilteredTransactions = new List<TransactionReceipt>();//[]*TransactionReceipt
	// The filtered transactions in this block when filtering has been applied,
	// (i.e. `filtering_applied = true`). This will be only the transactions
	// that matched the include filter CEL expression and did NOT match the exclude
	// filter CEL expression.
	//
	// Use the helper getter method `Transactions()` to automatically the correct
	// field (`unfiltered_transaction` when `filtering_applied == false` and
	// `filtered_transactions` when `filtering_applied == true`).
//	public TransactionReceipt[] FilteredTransactions = Array.Empty<TransactionReceipt>();//[]*TransactionReceipt
	// Number of transaction executed within this block when no filtering
	// is applied (`filtering_applied == false`).
//	public uint UnfilteredTransactionCount = 0;
	// Number of transaction that were successfully executed within this block that are found in
	// the `filtered_transactions` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
//	public uint FilteredTransactionCount = 0;
	// The unfiltered implicit transaction ops in this block when NO filtering has been applied,
	// (i.e. `filtering_applied = false`). When filtering has been applied on this block,
	// (i.e. `filtering_applied = true`), this field will be set to `nil` and instead, the
	// `filtered_implicit_transaction_ops` will be populated with only filtered implicit
	// transaction ops.
	//
	// Use the helper getter method `ImplicitTransactionOps()` to automatically pick the correct
	// field to use (`unfiltered_implicit_transaction_ops` when `filtering_applied == false` and
	// `filtered_implicit_transaction_ops` when `filtering_applied == true`).
	public IList<TrxOp> UnfilteredImplicitTransactionOps = new List<TrxOp>();//[]*TrxOp
	// The filtered implicit transaction ops in this block when filtering has been applied,
	// (i.e. `filtering_applied = true`). This will be only the implicit transaction ops
	// that matched the include filter CEL expression and did NOT match the exclude
	// filter CEL expression.
	//
	// Use the helper getter method `ImplicitTransactionOps()` to automatically the correct
	// field (`unfiltered_implicit_transaction_ops` when `filtering_applied == false` and
	// `filtered_implicit_transaction_ops` when `filtering_applied == true`).
//	public TrxOp[] FilteredImplicitTransactionOps = Array.Empty<TrxOp>();//[]*TrxOp
	// The unfiltered transaction traces in this block when NO filtering has been applied,
	// (i.e. `filtering_applied = false`). When filtering has been applied on this block,
	// (i.e. `filtering_applied = true`), this field will be set to `nil` and instead, the
	// `filtered_transaction_traces` will be populated with only filtered transactions.
	//
	// Use the helper getter method `TransactionTraces()` to automatically pick the correct
	// field to use (`unfiltered_transaction_traces` when `filtering_applied == false` and
	// `filtered_transaction_traces` when `filtering_applied == true`).
	public IList<TransactionTrace> UnfilteredTransactionTraces = new List<TransactionTrace>();//[]*TransactionTrace // THIS
	// The filtered transaction traces in this block when filtering has been applied,
	// (i.e. `filtering_applied = true`). This will be only the transaction trace
	// that matched the include filter CEL expression and did NOT match the exclude
	// filter CEL expression.
	//
	// Use the helper getter method `TransactionTraces()` to automatically pick the correct
	// field to use (`unfiltered_transaction_traces` when `filtering_applied == false` and
	// `filtered_transaction_traces` when `filtering_applied == true`).
//	public TransactionTrace[] FilteredTransactionTraces = Array.Empty<TransactionTrace>();//[]*TransactionTrace
	// Number of transaction trace executed within this block when no filtering
	// is applied (`filtering_applied == false`).
//	public uint UnfilteredTransactionTraceCount = 0;
	// Number of transaction trace that were successfully executed within this block that are found in
	// the `filtered_transaction_traces` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
//	public uint FilteredTransactionTraceCount = 0;
	// Number of top-level actions that were successfully executed within this block when no filtering
	// is applied (`filtering_applied == false`).
//	public uint UnfilteredExecutedInputActionCount = 0;
	// Number of top-level actions that were successfully executed within this block that are found in
	// the `filtered_transaction_traces` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
//	public uint FilteredExecutedInputActionCount = 0;
	// Number of actions that were successfully executed within this block when no filtering
	// is applied (`filtering_applied == false`).
//	public uint UnfilteredExecutedTotalActionCount = 0;
	// Number of actions that were successfully executed within this block that are found in
	// the `filtered_transaction_traces` array. This field is populated only when the flag
	// `filtering_applied` is `true`.
//	public uint FilteredExecutedTotalActionCount = 0;
	// This was a single string element representing a public key (eos-go#ecc.PublicKey).
	// It has been replaced by `valid_block_signing_authority_v2`.
	public PublicKey BlockSigningKey = PublicKey.Empty;//string
	// This was a list of `{name, publicKey}` elements, each block being signed by a single key,
	// the schedule was simply a list of pair, each pair being the producer name and it's public key
	// used to sign the block.
//	public ProducerSchedule ActiveScheduleV1 = new ProducerSchedule();//*ProducerSchedule
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
	public BlockSigningAuthorityVariant? ValidBlockSigningAuthority;//*BlockSigningAuthority
	// This repleaces the old type `ProducerSchedule` for the `active_schedule`
	// field. This was only a type change in EOSIO 2.0, the field's name remained
	// the same.
	//
	// This is the new schedule data layout which is richer than it's oldest
	// counterpart. The inner element for a producer can then be composed with
	// multiple keys, each with their own weight and the threshold required to
	// accept the block signature.
	public ProducerAuthoritySchedule ActiveSchedule = new();//*ProducerAuthoritySchedule
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
//	public bool FilteringApplied = false;//bool
	// The CEL filter expression used to include transaction in `filtered_transaction_traces` array, works
	// in combination with `filtering_exclude_filter_expr` value.
//	public string FilteringIncludeFilterExpr = string.Empty;//string
	// The CEL filter expression used to exclude transaction in `filtered_transaction_traces` array, works
	// in combination with `filtering_include_filter_expr` value.
//	public string FilteringExcludeFilterExpr = string.Empty;//string
	// The CEL filter expression used to include system actions, required by some systems, works
	// in combination with the two other filters above.
//	public string FilteringSystemActionsIncludeFilterExpr = string.Empty;//string

	internal object ToJsonString(JsonSerializerOptions? jsonSerializerOptions = null)
	{
		return JsonSerializer.Serialize(this, jsonSerializerOptions ?? new JsonSerializerOptions()
		{
			IncludeFields = true,
			IgnoreReadOnlyFields = false
		});
	}

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