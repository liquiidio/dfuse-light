using System.Text.Json;
using DeepReader.Types;
using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Helpers;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Toolkit.HighPerformance;
using Serilog;
namespace DeepReader.Classes;

public class ParseCtx
{
    private Block _block;

    private long _activeBlockNum;

    private TransactionTrace _trx;

    private readonly IList<CreationOp> _creationOps;

    private readonly bool _traceEnabled;

    private readonly string[] _supportedVersions = {"mandel", "13" };

    private AbiDecoder _abiDecoder;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

    public ParseCtx(AbiDecoder abiDecoder)
    {
        _block = new Block();
        _activeBlockNum = 0;
        _trx = new TransactionTrace();
        _creationOps = new List<CreationOp>();
        _traceEnabled = true;
        _abiDecoder = abiDecoder;
    }

    public ParseCtx(Block block, long activeBlockNum, TransactionTrace trx, List<CreationOp> creationOps, bool traceEnabled, string[] supportedVersions, AbiDecoder abiDecoder)
    {
        _block = block;
        _activeBlockNum = activeBlockNum;
        _trx = trx;
        _creationOps = creationOps;
        _traceEnabled = traceEnabled;
        _supportedVersions = supportedVersions;
        _abiDecoder = abiDecoder;
    }

    public void ResetBlock(ObjectPool<Block> blockPool, bool returnBlock)
	{
		// The nodeos bootstrap phase at chain initialization happens before the first block is ever
		// produced. As such, those operations needs to be attached to initial block. Hence, let's
		// reset recorded ops only if a block existed previously.
		if (_activeBlockNum != 0)
        {
            ResetTrx();
        }

        if(returnBlock)
            blockPool.Return(_block);

        _block = blockPool.Get();
    }

	public void ResetTrx()
    {
        _trx = new TransactionTrace();
        _creationOps.Clear();
    }

	public void RecordCreationOp(CreationOp operation)
    {
        _creationOps.Add(operation);
    }

	public void RecordDbOp(DbOp operation)
    {
        _trx.DbOps.Add(operation);
    }

	public void RecordDTrxOp(DTrxOp transaction)
    {
        _trx.DtrxOps.Add(transaction);
   
        if (transaction.Operation == DTrxOpOperation.FAILED)
        {
            RevertOpsDueToFailedTransaction();
        }
	}

    public void RecordFeatureOp(FeatureOp operation)
    {
        _trx.FeatureOps.Add(operation);
    }

    public void RecordPermOp(PermOp operation)
    {
        _trx.PermOps.Add(operation);
    }

    public void RecordRamOp(RamOp operation)
    {
        _trx.RamOps.Add(operation);
    }

    public void RecordRamCorrectionOp(RamCorrectionOp operation)
    {
        _trx.RamCorrectionOps.Add(operation);
    }

    public void RecordRlimitOp(RlimitOp operation)
	{
		if (operation is RlimitConfig || operation is RlimitState)
        {
            _block.RlimitOps.Add(operation);
        }
		else if (operation is RlimitAccountLimits || operation is RlimitAccountUsage) {
			_trx.RlimitOps.Add(operation);
	    }
	}

    public void RecordTableOp(TableOp operation)
    {
        _trx.TableOps.Add(operation);
    }

    public void RecordTrxOp(TrxOp operation)
    {
        _block.UnfilteredImplicitTransactionOps.Add(operation);
    }

    public void RecordTransaction(TransactionTrace trace)
    {
        var failedTrace = trace.FailedDtrxTrace;
        if (failedTrace != null) {
	        // Having a `FailedDtrxTrace` means the `trace` we got is an `onerror` handler.
	        // In this block, we perform all the logic to correctly record the `onerror`
	        // handler trace and the actual deferred transaction trace that failed.

	        // The deferred transaction removal RAM op needs to be attached to the failed trace, not the onerror handler
            _trx.RamOps = TransferDeferredRemovedRamOp(_trx.RamOps, failedTrace);

	        // The only possibilty to have failed deferred trace, is when the deferred execution
	        // resulted in a subjetive failure, which is really a soft fail. So, when the receipt is
	        // not set, let's re-create it here with soft fail status only.
	        failedTrace.Receipt ??= new TransactionReceiptHeader()
            {
                Status = TransactionStatus.SOFTFAIL
            };

            // We add the failed deferred trace first, before the "real" trace (the `onerror` handler)
            // since it was ultimetaly ran first. There is no ops possible on the trace expect the
            // transferred RAM op, so it's all good to attach it directly.
            _block.UnfilteredTransactionTraces.Add(failedTrace);

            // TODO
	        /*
	        if err := ctx.abiDecoder.processTransaction(failedTrace); err != nil {
                return fmt.Errorf("abi decoding failed trace: %w", err)
	        }
	        */

            // When the `onerror` `trace` receipt is `soft_fail`, it means the `onerror` handler
            // succeed. But when it's `hard_fail` it means either no handler was defined, or the one
            // defined failed to execute properly. So in the `hard_fail` case, let's reset all ops.
            // However, we do keep `RLimitOps` as they seems to be billed regardeless of transaction
            // execution status
            if (trace.Receipt == null || trace.Receipt.Status == TransactionStatus.HARDFAIL)
            {
                RevertOpsDueToFailedTransaction();
            }
        }

        // All this stiching of ops into trace must be performed after `if` because the if can revert them all
        var creationTreeRoots = CreationTree.ComputeCreationTree((IReadOnlyList<CreationOp>)_creationOps);

        trace.CreationTree = CreationTree.ToFlatTree(creationTreeRoots);
        trace.DtrxOps = _trx.DtrxOps;
        trace.DbOps = _trx.DbOps;
        trace.FeatureOps = _trx.FeatureOps;
        trace.PermOps = _trx.PermOps;
        trace.RamOps = _trx.RamOps;
        trace.RamCorrectionOps = _trx.RamCorrectionOps;
        trace.RlimitOps = _trx.RlimitOps;
        trace.TableOps = _trx.TableOps;

        _block.UnfilteredTransactionTraces.Add(trace);

        AbiDecoder.ProcessTransactionTrace(trace);

        ResetTrx();
    }

    private IList<RamOp> TransferDeferredRemovedRamOp(ICollection<RamOp> initialRamOps, TransactionTrace target)
    {
        IList<RamOp> filteredRamOps = new List<RamOp>();
        foreach (var ramOp in initialRamOps)
        {
            if (ramOp.Namespace == RamOpNamespace.DEFERRED_TRX && ramOp.Action == RamOpAction.REMOVE)
            {
                target.RamOps.Add(ramOp);
            } else
            {
                filteredRamOps.Add(ramOp);
            }            
        }

        return filteredRamOps;
    }

    public void RevertOpsDueToFailedTransaction() 
    {
        // We must keep the deferred removal, as this RAM changed is **not** reverted by nodeos, unlike all other ops
        // as well as the RLimitOps, which happens at a location that does not revert.
        var toRestoreRlimitOps = _trx.RlimitOps;

        RamOp? deferredRemovalRamOp = null;// = new RAMOp();

        foreach (var trxRamOp in _trx.RamOps)
        {
            if (trxRamOp.Namespace == RamOpNamespace.DEFERRED_TRX && trxRamOp.Action == RamOpAction.REMOVE)
            {
                deferredRemovalRamOp = trxRamOp;
                break;
            }
        }

        ResetTrx();
        _trx.RlimitOps = toRestoreRlimitOps;
        if (deferredRemovalRamOp != null)
        {
            _trx.RamOps = new List<RamOp>() { deferredRemovalRamOp };
        }
    }

    public RamOp[] TransferDeferredRemovedRamOp(RamOp[] initialRamOps, TransactionTrace target)
    {
        List<RamOp> filteredRamOps = new List<RamOp>();
        foreach (var initialRamOp in initialRamOps)
        {
			if (initialRamOp.Namespace == RamOpNamespace.DEFERRED_TRX && initialRamOp.Action == RamOpAction.REMOVE)
            {
                target.RamOps.Add(initialRamOp);
            }
            else
            {
                filteredRamOps.Add(initialRamOp);
            }
		}
        return filteredRamOps.ToArray();
    }

	// Line format:
	//   START_BLOCK ${block_num}
    public void ReadStartBlock(in IList<StringSegment> chunks, ObjectPool<Block> blockPool)
    {
        if (chunks.Count != 3)
        {
            throw new Exception($"expected 3 fields, got {chunks.Count}");
        }

        var blockNum = Int64.Parse(chunks[2].AsSpan);

        ResetBlock(blockPool, false);// TODO either ResetBlock in ReadAcceptedBlock or in ReadStartBlock is unnecessary
        _activeBlockNum = blockNum;

        AbiDecoder.StartBlock(blockNum);
    }

    /// <summary>
    /// plugins/chain_plugin/chain_plugin.cpp:1162
    /// [ACCEPTED_BLOCK ${block_num} ${block_state_hex}]
    /// </summary>
    /// <param name="chunks"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    public Block ReadAcceptedBlock(in IList<StringSegment> chunks, ObjectPool<Block> blockPool) {
        if (chunks.Count != 4)
        {
            throw new Exception($"expected 4 fields, got {chunks.Count}");
        }

        var blockNum = Int64.Parse(chunks[2].AsSpan);
        if(blockNum == _activeBlockNum + 1)
        {
            _activeBlockNum++;
        }

        if (_activeBlockNum != blockNum)
        {
            Log.Information($"block_num {blockNum} doesn't match the active block num {_activeBlockNum}");
        }

        var blockState = DeepMindDeserializer.DeepMindDeserializer.Deserialize<BlockState>(Decoder.HexToBytes(chunks[3]));

        _block.Id = blockState.Id;
        _block.Number = blockState.BlockNum;
        _block.Version = 1;
        _block.Header = blockState.Header;
        _block.DposIrreversibleBlocknum = blockState.DPoSIrreversibleBlockNum;
        _block.DposProposedIrreversibleBlocknum = blockState.DPoSProposedIrreversibleBlockNum;
        _block.Validated = blockState.Validated;
        _block.BlockrootMerkle = blockState.BlockrootMerkle;
        _block.ProducerToLastProduced = blockState.ProducerToLastProduced;
        _block.ProducerToLastImpliedIrb = blockState.ProducerToLastImpliedIrb;
        _block.ActivatedProtocolFeatures = blockState.ActivatedProtocolFeatures;
        _block.PendingSchedule = blockState.PendingSchedule;
        _block.ActiveSchedule = blockState.ActiveSchedule;
        _block.ValidBlockSigningAuthority = blockState.ValidBlockSigningAuthority;
        _block.BlockSigningKey = blockState.ValidBlockSigningAuthority is BlockSigningAuthorityV0 blockSigningAuthority ? blockSigningAuthority.Keys.FirstOrDefault()?.Key ?? PublicKey.TypeEmpty : PublicKey.TypeEmpty;
        _block.ConfirmCount = blockState.ConfirmCount.ToArray();
        _block.BlockExtensions = blockState.Block?.BlockExtensions ?? Array.Empty<KeyValuePair<ushort, char[]>>();
        _block.ProducerSignature = blockState.Block?.ProducerSignature ?? Signature.TypeEmpty;
        _block.UnfilteredTransactions = blockState.Block?.Transactions.ToList() ?? new List<TransactionReceipt>();

        // this is hydrator.hydrateblock ... 
        
        //block.Id = blockState.BlockID;
        //block.Number = blockState.BlockNum;
        // Version 1: Added the total counts (ExecutedInputActionCount, ExecutedTotalActionCount,
        // TransactionCount, TransactionTraceCount)
        //block.Version = 1;
        //block.Header = blockState.Header;
        //if (blockState.Block != null)
        //{
        //    block.BlockExtensions = blockState.Block.BlockExtensions;
        //    block.ProducerSignature = blockState.Block.ProducerSignature;
        //}
        //block.DposIrreversibleBlocknum = blockState.DPoSIrreversibleBlockNum;
        //block.DposProposedIrreversibleBlocknum = blockState.DPoSProposedIrreversibleBlockNum;
        //block.Validated = blockState.Validated;
        //block.BlockrootMerkle = blockState.BlockrootMerkle;
        //block.ProducerToLastProduced = blockState.ProducerToLastProduced;
        //block.ProducerToLastImpliedIrb = blockState.ProducerToLastImpliedIRB;
        //block.ActivatedProtocolFeatures = blockState.ActivatedProtocolFeatures;

        //for (int i = 0; i < blockState.ConfirmCount.Length; i++)
        //{
        //    block.ConfirmCount[i] = blockState.ConfirmCount[i];
        //}

        //block.PendingSchedule = blockState.PendingSchedule;

        // Specific versions handling

//        var blockSigningKey = blockState.BlockSigningKeyV1; 
        // Only in EOSIO 1.x
        //if (blockSigningKey != null)
        //{
        //    block.BlockSigningKey = Encoding.ASCII.GetString(blockSigningKey);
        //}

        /*block.ActiveSchedule = blockState.ActiveSchedule*/ // TODO below comments where used instead of this

        //if (schedule.V1 != null)
        //{
        //    block.ActiveScheduleV1 = schedule.V1;
        //}

        // Only in EOSIO 2.x

        //block.ValidBlockSigningAuthority = blockState.ValidBlockSigningAuthority;

        //if (schedule.V2 != null)
        //{
        //    block.ActiveScheduleV2 = schedule.V2;
        //}

        // End (versions)

//        if(blockState.Block != null)
//        {
////            block.UnfilteredTransactionCount = (uint)blockState.Block.Transactions.Length;
//            block.UnfilteredTransactions = blockState.Block.Transactions;
//        }

//        block.UnfilteredTransactionTraceCount = (uint) block.UnfilteredTransactionTraces.Count;

        for (int idx = 0; idx < _block.UnfilteredTransactionTraces.Count; idx++)
        {
            var el = _block.UnfilteredTransactionTraces[idx];
            el.Index = (ulong)idx;
            el.BlockTime = 0;// TODO block.Header.Timestamp;
            el.ProducerBlockId = _block.Id;
            el.BlockNum = _block.Number;

            //foreach (var actionTrace in el.ActionTraces)
            //{
	           // if (actionTrace.IsInput())
	           // {
		          //  block.UnfilteredExecutedInputActionCount++;
	           // }
            //}
        }

//	        zlog.Debug("blocking until abi decoder has decoded every transaction pushed to it")

        AbiDecoder.EndBlock(_block);
        var block = _block;
        ResetBlock(blockPool, false);// TODO either ResetBlock in ReadAcceptedBlock or in ReadStartBlock is unnecessary
        return block;
    }

    //private PendingProducerSchedule PendingScheduleToDEOS(PendingSchedule blockStatePendingSchedule)
    //{
    //    var pendingProducerSchedule = new PendingProducerSchedule()
    //    {
	   //     ScheduleLibNum = blockStatePendingSchedule.ScheduleLIBNum,
	   //     ScheduleHash = blockStatePendingSchedule.ScheduleHash,
    //    };

    //    /// Specific versions handling

    //    // Only in EOSIO 1.x
    //    if (blockStatePendingSchedule.Schedule.V1 != null)
    //    {
	   //     pendingProducerSchedule.ScheduleV1 = blockStatePendingSchedule.Schedule.V1;
    //    }

    //    // Only in EOSIO 2.x
    //    if (blockStatePendingSchedule.Schedule.V2 != null)
    //    {
	   //     pendingProducerSchedule.ScheduleV2 = blockStatePendingSchedule.Schedule.V2;
    //    }

    //    // End (versions)
    //    return pendingProducerSchedule;
    //}

    //private ActivatedProtocolFeatures ActivatedProtocolFeaturesToDEOS(ProtocolFeatureActivationSet blockStateActivatedProtocolFeatures)
    //{

    //    return new ActivatedProtocolFeatures()
    //    {
    //        // TODO !! ProtocolFeatures multi-dim array?
    //        ProtocolFeatures = blockStateActivatedProtocolFeatures.ProtocolFeatures.Length > 0 ? checksumsToBytesSlices(blockStateActivatedProtocolFeatures.ProtocolFeatures[0]) : Array.Empty<byte[]>()
    //    };
    //}

    //private ProducerToLastImpliedIRB[] ProducerToLastImpliedIrbToDEOS(PairAccountNameBlockNum[] blockStateProducerToLastImpliedIrb)
    //{
    //    var producerToLastImpliedIRB = new ProducerToLastImpliedIRB[blockStateProducerToLastImpliedIrb.Length];
    //    for (int i = 0; i < blockStateProducerToLastImpliedIrb.Length; i++)
    //    {
	   //     producerToLastImpliedIRB[i] = new ProducerToLastImpliedIRB()
	   //     {
		  //      Name = blockStateProducerToLastImpliedIrb[i].AccountName,
		  //      LastBlockNumProduced = blockStateProducerToLastImpliedIrb[i].BlockNum
	   //     };
    //    }
    //    return producerToLastImpliedIRB;
    //}

    //private ProducerToLastProduced[] ProducerToLastProducedToDEOS(PairAccountNameBlockNum[] blockStateProducerToLastProduced)
    //{
    //    var producerToLastProduced = new ProducerToLastProduced[blockStateProducerToLastProduced.Length];
    //    for (int i = 0; i < blockStateProducerToLastProduced.Length; i++)
    //    {
	   //     producerToLastProduced[i] = new ProducerToLastProduced()
	   //     {
		  //      Name = blockStateProducerToLastProduced[i].AccountName,
		  //      LastBlockNumProduced = blockStateProducerToLastProduced[i].BlockNum
	   //     };
    //    }

    //    return producerToLastProduced;
    //}

    //private BlockRootMerkle BlockrootMerkleToDEOS(IncrementalMerkle merkle)
    //{
    //    return new BlockRootMerkle()
    //    {
	   //     NodeCount = (uint)merkle.NodeCount,
	   //     ActiveNodes = checksumsToBytesSlices(merkle.ActiveNodes)
    //    };
    //}

    //private byte[][] ChecksumsToBytesSlices(byte[] merkleActiveNodes)
    //{
    //    // TODO
    //    return new[] { merkleActiveNodes };
    //}

    //private Extension[] ExtensionsToDEOS(Extension[] signedBlockBlockExtensions)
    //{
    //    // ?! TODO
    //    return signedBlockBlockExtensions;
    //}

    //private BlockHeader BlockHeaderToDEOS(SignedBlock signedBlock)
    //{
    //    var blockHeader = new BlockHeader()
    //    {
	   //     Timestamp = signedBlock.Timestamp,
	   //     Producer = signedBlock.Producer,
	   //     Confirmed = signedBlock.Confirmed,
	   //     Previous = signedBlock.Previous,
	   //     TransactionMroot = signedBlock.TransactionMroot,
	   //     ActionMroot = signedBlock.ActionMroot,
	   //     ScheduleVersion = signedBlock.ScheduleVersion,
	   //     HeaderExtensions = ExtensionsToDEOS(signedBlock.HeaderExtensions),
    //    };

    //    if (blockHeader.NewProducers != null)
    //    {
	   //     blockHeader.NewProducers = blockHeader.NewProducers;
    //    }
        
    //    return blockHeader;
    //}

    // Line format:
    //   APPLIED_TRANSACTION ${block_num} ${trace_hex}
    public void ReadAppliedTransaction(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 4)
        {
            throw new Exception($"expected 4 fields, got {chunks.Count}");
        }

        var blockNum = Int32.Parse(chunks[2].AsSpan);

        if (_activeBlockNum != blockNum)
        {
//            throw new Exception($"saw transactions from block {blockNum} while active block is {ActiveBlockNum}");
            Log.Information($"saw transactions from block {blockNum} while active block is {_activeBlockNum}");
        }

        var trxTrace = DeepMindDeserializer.DeepMindDeserializer.Deserialize<TransactionTrace>(Decoder.HexToBytes(chunks[3]));

        RecordTransaction(trxTrace);
    }

    // Line formats:
    //  CREATION_OP ROOT ${action_id}
    //  CREATION_OP NOTIFY ${action_id}
    //  CREATION_OP INLINE ${action_id}
    //  CREATION_OP CFA_INLINE ${action_id}
    public void ReadCreationOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 4)
        {
            throw new Exception($"expected 4 fields, got {chunks.Count}");
        }

        var kind = chunks[2];
        if (kind != "ROOT" && kind != "NOTIFY" && kind != "INLINE" && kind != "CFA_INLINE")
        {
            throw new Exception($"kind must be one of ROOT, NOTIFY, CFA_INLINE or INLINE, got: {kind}");
        }

        var actionIndex = Int32.Parse(chunks[3].AsSpan);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        RecordCreationOp(new CreationOp()
        {
            Kind = Enum.Parse<CreationOpKind>(kind),
            // FIXME: this index is 0-based, whereas `action_ordinal` is 1-based, where 0 means a virtual root node.
            // This is a BIG problem as now we unpack the traces and simply keep that `action_ordinal` field.. so in `eosws`, we need to re-map all of this together.
            // Perhaps we can simply ditch all of this since we'll have the `closest unnotified ancestor`,.. and we could *NOT* compute our own thing anymore.. and always use theirs..
            // then simply re-map their model into ours at the edge (in `eosws`).
            ActionIndex = actionIndex,
        });
    }

    // Line formats:
    //   DB_OP INS ${action_id} ${payer} ${table_code} ${scope} ${table_name} ${primkey} ${ndata}
    //   DB_OP UPD ${action_id} ${opayer}:${npayer} ${table_code} ${scope} ${table_name} ${primkey} ${odata}:${ndata}
    //   DB_OP REM ${action_id} ${payer} ${table_code} ${scope} ${table_name} ${primkey} ${odata}
    public void ReadDbOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 10)
        {
            throw new Exception($"expected 10 fields, got {chunks.Count}");
        }

        var actionIndex = Int32.Parse(chunks[3].AsSpan);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        var op = Enum.Parse<DbOpOperation>(chunks[2]);
        ReadOnlyMemory<byte> oldData = default, newData = default;
        Name oldPayer = Name.TypeEmpty, newPayer = Name.TypeEmpty;

        switch (op)
        {
	        case DbOpOperation.INS:
                newData = chunks[9].AsMemory.Cast<char, byte>();
                newPayer = SerializationHelper.CharSpanToName(chunks[4].AsSpan);
				break;
            case DbOpOperation.UPD:
                var dataChunks = chunks[9].Split(':');
	            if (dataChunks.Count != 2)
                {
                    throw new Exception("should have old and new data in field 8, found only one");
                }

                oldData = dataChunks[0].AsMemory.Cast<char, byte>();
                newData = dataChunks[1].AsMemory.Cast<char, byte>();

                var payerChunks = chunks[4].Split(':');
	            if (payerChunks.Count != 2)
                {
                    throw new Exception("should have two payers in field 3, separated by a ':', found only one");
                }

                oldPayer = SerializationHelper.CharSpanToName(payerChunks[0].AsSpan);
                newPayer = SerializationHelper.CharSpanToName(payerChunks[1].AsSpan);
				break;
            case DbOpOperation.REM:
                oldData = chunks[9].AsMemory.Cast<char, byte>();
                oldPayer = SerializationHelper.CharSpanToName(chunks[4].AsSpan);
				break;
            default:
                throw new Exception($"unknown operation: {chunks[2]}");
        }

        RecordDbOp(new DbOp()
        {
            Operation = op,
            ActionIndex = (uint) actionIndex,
            OldPayer = oldPayer,
            NewPayer = newPayer,
            Code = SerializationHelper.CharSpanToName(chunks[5].AsSpan),
            Scope = SerializationHelper.CharSpanToName(chunks[6].AsSpan),
            TableName = SerializationHelper.CharSpanToName(chunks[7].AsSpan),
            PrimaryKey = (string)chunks[8]!,
            OldData = oldData,
            NewData = newData,
        });
    }

    // Line formats:
    //   DTRX_OP MODIFY_CANCEL ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP MODIFY_CREATE ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP CREATE        ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP CANCEL        ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP PUSH_CREATE   ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    public void ReadCreateOrCancelDTrxOp(string tag, IList<StringSegment> chunks)
    {
        if (chunks.Count != 12)
        {
            throw new Exception($"expected 12 fields, got {chunks.Count}");
        }

        var opString = chunks[2];
        var rawOp = Enum.Parse<DTrxOpOperation>(opString, true);//pbcodec.DTrxOp_Operation_value["OPERATION_" + opString]);

        var op = rawOp; //pbcodec.DTrxOp_Operation(rawOp);

        var actionIndex = Int32.Parse(chunks[3].AsSpan);

        var signedTrx = DeepMindDeserializer.DeepMindDeserializer.Deserialize<SignedTransaction>(Decoder.HexToBytes(chunks[11]));

        try
        {
            RecordDTrxOp(new DTrxOp()
            {
                Operation = op,
                ActionIndex = (uint)actionIndex,
                Sender = SerializationHelper.CharSpanToName(chunks[4].AsSpan),
                SenderId = chunks[5].AsMemory,
                Payer = SerializationHelper.CharSpanToName(chunks[6].AsSpan),
                PublishedAt = DateTimeOffset.Parse(chunks[7]),
                DelayUntil = DateTimeOffset.Parse(chunks[8]),
                ExpirationAt = DateTimeOffset.Parse(chunks[9]),
                TransactionId = chunks[10].AsSpan,
                Transaction = signedTrx,
            });
        }
        catch (Exception)
        {
            Console.WriteLine($"SenderId {chunks[5]}");
            throw;
        }

        //_ = Task.Run(() => AbiDecoder.ProcessSignedTransaction(signedTrx));
    }

    // Line format:
    //   DTRX_OP FAILED ${action_id}
    public void ReadFailedDTrxOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 4)
        {
            throw new Exception($"expected 4 fields, got {chunks.Count}");
        }

        var actionIndex = Int32.Parse(chunks[3].AsSpan);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        RecordDTrxOp(new DTrxOp()
        {
            Operation = DTrxOpOperation.FAILED,
            ActionIndex = (uint) actionIndex,
        });
    }

    // Line formats:
    //   FEATURE_OP ACTIVATE ${feature_digest} ${feature}
    public void ReadFeatureOpActivate(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 6)
        {
            throw new Exception($"expected 6 fields, got {chunks.Count}");
        }

        var feature = JsonSerializer.Deserialize<Feature>(chunks[5], _jsonSerializerOptions)!;
        // TODO does this work?
        //err:= json.Unmarshal(json.RawMessage(chunks[3]), &feature)
        /*if err != nil {
	        return fmt.Errorf("unmashall new feature data: %s", err)
        }*/

        RecordFeatureOp(new FeatureOp()
        {
            Kind = (string)chunks[3]!,
            FeatureDigest = chunks[4].AsMemory,
            Feature = feature,
        });

        Console.WriteLine("FeatureOpKind: " + chunks[3]);   // TODO remove once we know all the kinds
    }

    // Line formats:
    //   FEATURE_OP PRE_ACTIVATE ${action_id} ${feature_digest} ${feature}
    public void ReadFeatureOpPreActivate(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 7)
        {
            throw new Exception($"expected 7 fields, got {chunks.Count}");
        }

        var actionIndex = Int32.Parse(chunks[4].AsSpan);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        var feature = JsonSerializer.Deserialize<Feature>(chunks[4], _jsonSerializerOptions)!;
        // TODO does this work?
        /*err = json.Unmarshal(json.RawMessage(chunks[4]), &feature)
        if err != nil {
	        return fmt.Errorf("unmashall new feature data: %s", err)
        }*/

        RecordFeatureOp(new FeatureOp()
        {
            Kind = (string)chunks[3]!,
            ActionIndex = (uint) actionIndex,
            FeatureDigest = chunks[5].AsMemory,
            Feature = feature,
        });
    }

    // Line formats: (the `[...]` represents optional fields)
    //   PERM_OP INS ${action_id} [${permission_id}] ${data}
    //   PERM_OP UPD ${action_id} [${permission_id}] ${data}
    //   PERM_OP REM ${action_id} [${permission_id}] ${data} <-- {"old": <old>, "new": <new>}
    public void ReadPermOp(in IList<StringSegment> chunks)
    {
        var actionIndex = Int32.Parse(chunks[3].AsSpan);

        var dataChunk = chunks[4];
        ulong permissionId = 0;

        // A `PERM_OP` with 5 fields have ["permission_id"] field in index #3 set and data chunk is actually index #4
        if (chunks.Count == 6)
        {
            permissionId = UInt64.Parse(chunks[4]);
            dataChunk = chunks[5];
        }

        var op = Enum.Parse<PermOpOperation>(chunks[2]);
        ReadOnlySpan<char> oldData = default, newData = default;

        switch (op) {
	        case PermOpOperation.INS:
                //var feature = JsonSerializer.Deserialize<PermissionObject>(chunks[3]);
                newData = dataChunk.AsSpan;
                //                newData = Encoding.ASCII.GetBytes(dataChunk);
                break;
            case PermOpOperation.UPD:
                //   var oldJSONResult = gjson.Get(dataChunk, "old")
                // TODO deserialize directly?
                JsonDocument jsonDocument = JsonDocument.Parse(dataChunk.AsMemory);
                if (!jsonDocument.RootElement.TryGetProperty("old", out var oldJsonResult))
                {
                    throw new Exception(
                        $"a PERM_OP UPD should JSON data should have an 'old' field, found none in: {dataChunk}");
                }

	            if (!jsonDocument.RootElement.TryGetProperty("new", out var newJsonResult))
                {
                    throw new Exception(
                        $"a PERM_OP UPD should JSON data should have an 'new' field, found none in: {dataChunk}");
                }

                oldData = oldJsonResult.GetRawText().AsSpan();
    	        newData = newJsonResult.GetRawText().AsSpan();
                break;
            case PermOpOperation.REM:
                oldData = dataChunk.AsSpan;
                break;
            default:
                throw new Exception($"unknown PERM_OP op: {chunks[2]}");
        }

        var permOp = new PermOp()
        {
            Operation = op,
            ActionIndex = (uint) actionIndex,
        };

        if (newData.Length > 0)
        {
            try
            {
                var newPerm = JsonSerializer.Deserialize<PermissionObject>(newData, _jsonSerializerOptions)!;

                permOp.NewPerm = newPerm;
                permOp.NewPerm.UsageId = permissionId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(newData.ToString());
                throw;
            }
        }

        if (oldData.Length > 0)
        {
            var oldPerm = JsonSerializer.Deserialize<PermissionObject>(oldData, _jsonSerializerOptions)!;
	        /*err = json.Unmarshal(oldData, &oldPerm)
	        if err != nil {
		        return fmt.Errorf("unmashal old perm data: %s", err)
	        }*/
            permOp.OldPerm = oldPerm;
            permOp.OldPerm.UsageId = permissionId;
        }

        RecordPermOp(permOp);
    }

    // Line format:
    //   RAM_OP ${action_index} ${unique_key} ${namespace} ${action} ${legacy_tag} ${payer} ${new_usage} ${delta}
    public void ReadRamOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 10)
        {
            throw new Exception($"expected 10 fields, got {chunks.Count}");
        }

        var actionIndex = UInt32.Parse(chunks[2]);

        var @namespace = Enum.Parse<RamOpNamespace>(chunks[4], true);

        var action = Enum.Parse<RamOpAction>(chunks[5], true);

        var operation = Enum.Parse<RamOpOperation>(chunks[6], true);

        var usage = UInt64.Parse(chunks[8]);

        var delta = Int64.Parse(chunks[9]);

        RecordRamOp(new RamOp()
        {
            ActionIndex = actionIndex,
            UniqueKey = chunks[3].Split(':'),
            Namespace = @namespace,
            Action = action,
            Operation = operation,
            Payer = SerializationHelper.CharSpanToName(chunks[7].AsSpan),
            Usage = usage,
            Delta = delta,
        });
    }

    // Line format:
    //  Version 12
    //    DEEP_MIND_VERSION ${major_version}
    //
    //  Version 13
    //    DEEP_MIND_VERSION ${major_version} ${minor_version}
    public void ReadDeepmindVersion(in IList<StringSegment> chunks)
    {
        var majorVersion = chunks[2];
        if (!InSupportedVersion((string)majorVersion!)) {
            throw new Exception(
                $"deep mind reported version {majorVersion}, but this reader supports only {string.Join(", ", _supportedVersions)}");
        }
        Log.Information($"read deep mind version {majorVersion}");
    }

    public bool InSupportedVersion(string majorVersion)
    {
        foreach (var supportedVersion in _supportedVersions)
        {
            if (majorVersion == supportedVersion)
            {
                return true;
            }
        }
        return false;
    }

    // Line format:
    //  Version 12
    //    ABIDUMP START
    //
    //  Version 13
    //    ABIDUMP START ${block_num} ${global_sequence_num}
    public void ReadAbiStart(in IList<StringSegment> chunks)
    {
        // RANGE 3!

        // TODO
        switch (chunks.Count) {
            case 5: // Version > 12 ?
                var blockNum = Int32.Parse(chunks[3].AsSpan);
                var globalSequence = UInt64.Parse(chunks[4].AsSpan);
                _abiDecoder.AbiDumpStart(blockNum, globalSequence);
                Log.Information($"read ABI start marker: block_num {blockNum} global_sequence {globalSequence}");
                break;
            default:
                throw new Exception($"expected to have either {{3}} or {{5}} fields, got {chunks.Count}");
            }
        //AbiDecoder.ResetCache();
    }

    // Line format:
    //  Version 12
    //    ABIDUMP ABI ${block_num} ${contract} ${base64_abi}
    //
    //  Version 13
    //    ABIDUMP ABI ${contract} ${base64_abi}
    public void ReadAbiDump(in IList<StringSegment> chunks)
    {
        // RANGE 3!

        ReadOnlySpan<char> contract = default, rawAbi = default;
        switch (chunks.Count) {
            case 3: // Version < 14?
                break;
	        case 5: // Version 14?
                contract = chunks[3].AsSpan;
                rawAbi = chunks[4].AsSpan;
                break;
            default:
                throw new Exception($"expected to have either {{3}} or {{5}} fields, got {chunks.Count}");
        }

        if (_traceEnabled)
        {
            Log.Information($"read initial ABI for contract {contract}");
        }

        _abiDecoder.AddInitialAbi(contract, rawAbi);
    }

    // Line format:
    //    ABIDUMP END
    public void AbiDumpEnd()
    {
        _abiDecoder.AbiDumpEnd();
    }

    // Line format:
    //   RAM_CORRECTION_OP ${action_id} ${correction_id} ${unique_key} ${payer} ${delta}
    public void ReadRamCorrectionOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 7)
        {
            throw new Exception($"expected 7 fields, got {chunks.Count}");
        }

        // We assume ${action_id} will always be 0, since called from onblock, so that's why we do not process it

        var delta = Int64.Parse(chunks[6].AsSpan);
        /*if err != nil {
	        return fmt.Errorf("delta not a valid number, got: %q", chunks[5])
        }*/

        RecordRamCorrectionOp(new RamCorrectionOp()
        {
            CorrectionId = UInt64.Parse(chunks[3]), // TODO
            UniqueKey = UInt64.Parse(chunks[4]), // TODO
            Payer = SerializationHelper.CharSpanToName(chunks[5].AsSpan),
            Delta = delta,
        });
    }

    // Line formats:
    //   RLIMIT_OP CONFIG         INS ${data}
    //   RLIMIT_OP CONFIG         UPD ${data}
    //   RLIMIT_OP STATE          INS ${data}
    //   RLIMIT_OP STATE          UPD ${data}
    //   RLIMIT_OP ACCOUNT_LIMITS INS ${data}
    //   RLIMIT_OP ACCOUNT_LIMITS UPD ${data}
    //   RLIMIT_OP ACCOUNT_USAGE  INS ${data}
    //   RLIMIT_OP ACCOUNT_USAGE  UPD ${data}
    public void ReadRlimitOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 5)
        {
            throw new Exception($"expected 5 fields, got {chunks.Count}");
        }

        var rlimitOpOperation = Enum.Parse<RlimitOpOperation>(chunks[3]);
        var rlimitOpKind = Enum.Parse<RlimitOpKind>(chunks[2]);

        RlimitOp op;

        switch (rlimitOpKind) {
	        case RlimitOpKind.CONFIG:
                op = JsonSerializer.Deserialize<RlimitConfig>(chunks[4], _jsonSerializerOptions)!;
                break;
            case RlimitOpKind.STATE:
                op = JsonSerializer.Deserialize<RlimitState>(chunks[4], _jsonSerializerOptions)!;
                break;
            case RlimitOpKind.ACCOUNT_LIMITS:
                op = JsonSerializer.Deserialize<RlimitAccountLimits>(chunks[4], _jsonSerializerOptions)!;
                break;
            case RlimitOpKind.ACCOUNT_USAGE:
                op = JsonSerializer.Deserialize<RlimitAccountUsage>(chunks[4], _jsonSerializerOptions)!;
                break;
            default:
                throw new Exception($"unknown kind: {chunks[2]}");
        }

        op.Operation = rlimitOpOperation;

        RecordRlimitOp(op);
    }

    // Line formats:
    //   TBL_OP INS ${action_id} ${code} ${scope} ${table} ${payer}
    //   TBL_OP REM ${action_id} ${code} ${scope} ${table} ${payer}
    public void ReadTableOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 8)
        {
            throw new Exception($"expected 8 fields, got {chunks.Count}");
        }

        RecordTableOp(new TableOp()
        {
            Operation = Enum.Parse<TableOpOperation>(chunks[2]),
            ActionIndex = (uint)Int32.Parse(chunks[3]),
            Payer = SerializationHelper.CharSpanToName(chunks[7].AsSpan),
            Code = SerializationHelper.CharSpanToName(chunks[4].AsSpan),
            Scope = SerializationHelper.CharSpanToName(chunks[5].AsSpan),
            TableName = SerializationHelper.CharSpanToName(chunks[6].AsSpan),
        });
    }

    // Line formats:
    //   TRX_OP CREATE onblock|onerror ${id} ${trx}
    public void ReadTrxOp(in IList<StringSegment> chunks)
    {
        if (chunks.Count != 6)
        {
            throw new Exception($"expected 6 fields, got {chunks.Count}");
        }

        var trx = DeepMindDeserializer.DeepMindDeserializer.Deserialize<SignedTransaction>(Decoder.HexToBytes(chunks[5]));

        RecordTrxOp(new TrxOp()
        {
            Operation = Enum.Parse<TrxOpOperation>(chunks[2]),
            Name = SerializationHelper.CharSpanToName(chunks[3].AsSpan), // "onblock" or "onerror"
            TransactionId = chunks[4].AsSpan, // the hash of the transaction
            Transaction = trx,
        });
    }
}