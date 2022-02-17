using System.Text.Json;
using DeepReader.Types;
using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using Serilog;
namespace DeepReader.Classes;

public class ParseCtx
{
    public Block Block;

    public long ActiveBlockNum = 1;

    public TransactionTrace Trx;

    public List<CreationOp> CreationOps;

    public bool TraceEnabled;

    public string[] SupportedVersions = new[] {"13"};

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

    public ParseCtx()
    {
        Block = new Block();
        ActiveBlockNum = 0;
        Trx = new TransactionTrace();
        CreationOps = new List<CreationOp>();
        TraceEnabled = true;
    }

    public ParseCtx(Block block, long activeBlockNum, TransactionTrace trx, List<CreationOp> creationOps, bool traceEnabled, string[] supportedVersions)
    {
        Block = block;
        ActiveBlockNum = activeBlockNum;
        Trx = trx;
        CreationOps = creationOps;
        TraceEnabled = traceEnabled;
        SupportedVersions = supportedVersions;
    }

    public void ResetBlock()
	{
		// The nodeos bootstrap phase at chain initialization happens before the first block is ever
		// produced. As such, those operations needs to be attached to initial block. Hence, let's
		// reset recorded ops only if a block existed previously.
		if (ActiveBlockNum != 0)
        {
            ResetTrx();
        }
		
        Block = new Block();
    }

	public void ResetTrx()
    {
        Trx = new TransactionTrace();
    }

	public void RecordCreationOp(CreationOp operation)
    {
        CreationOps.Add(operation);
    }

	public void RecordDbOp(DbOp operation)
    {
        Trx.DbOps.Add(operation);
    }

	public void RecordDTrxOp(DTrxOp transaction)
    {
        Trx.DtrxOps.Add(transaction);
   
        if (transaction.Operation == DTrxOpOperation.FAILED)
        {
            RevertOpsDueToFailedTransaction();
        }
	}

    public void RecordFeatureOp(FeatureOp operation)
    {
        Trx.FeatureOps.Add(operation);
    }

    public void RecordPermOp(PermOp operation)
    {
        Trx.PermOps.Add(operation);
    }

    public void RecordRamOp(RamOp operation)
    {
        Trx.RamOps.Add(operation);
    }

    public void RecordRamCorrectionOp(RamCorrectionOp operation)
    {
        Trx.RamCorrectionOps.Add(operation);
    }

    public void RecordRlimitOp(RlimitOp operation)
	{
		if (operation is RlimitConfig || operation is RlimitState)
        {
            Block.RlimitOps.Add(operation);
        }
		else if (operation is RlimitAccountLimits || operation is RlimitAccountUsage) {
			Trx.RlimitOps.Add(operation);
	    }
	}

    public void RecordTableOp(TableOp operation)
    {
        Trx.TableOps.Add(operation);
    }

    public void RecordTrxOp(TrxOp operation)
    {
        Block.UnfilteredImplicitTransactionOps.Add(operation);
    }

    public void RecordTransaction(TransactionTrace trace)
    {
        var failedTrace = trace.FailedDtrxTrace;
        if (failedTrace != null) {
	        // Having a `FailedDtrxTrace` means the `trace` we got is an `onerror` handler.
	        // In this block, we perform all the logic to correctly record the `onerror`
	        // handler trace and the actual deferred transaction trace that failed.

	        // The deferred transaction removal RAM op needs to be attached to the failed trace, not the onerror handler
            Trx.RamOps = TransferDeferredRemovedRamOp(Trx.RamOps, failedTrace);

	        // The only possibilty to have failed deferred trace, is when the deferred execution
	        // resulted in a subjetive failure, which is really a soft fail. So, when the receipt is
	        // not set, let's re-create it here with soft fail status only.
	        failedTrace.Receipt ??= new TransactionReceiptHeader()
            {
                Status = (byte) TransactionStatus.SOFTFAIL
            };

            // We add the failed deferred trace first, before the "real" trace (the `onerror` handler)
            // since it was ultimetaly ran first. There is no ops possible on the trace expect the
            // transferred RAM op, so it's all good to attach it directly.
            Block.UnfilteredTransactionTraces.Add(failedTrace);

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
            if (trace.Receipt == null || trace.Receipt.Status == (byte)TransactionStatus.HARDFAIL)
            {
                RevertOpsDueToFailedTransaction();
            }
        }

        // All this stiching of ops into trace must be performed after `if` because the if can revert them all
        var creationTreeRoots = CreationTree.ComputeCreationTree(CreationOps);

        trace.CreationTree = CreationTree.ToFlatTree(creationTreeRoots).ToArray();
        trace.DtrxOps = Trx.DtrxOps;
        trace.DbOps = Trx.DbOps;
        trace.FeatureOps = Trx.FeatureOps;
        trace.PermOps = Trx.PermOps;
        trace.RamOps = Trx.RamOps;
        trace.RamCorrectionOps = Trx.RamCorrectionOps;
        trace.RlimitOps = Trx.RlimitOps;
        trace.TableOps = Trx.TableOps;

        Block.UnfilteredTransactionTraces.Add(trace);

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
        var toRestoreRlimitOps = Trx.RlimitOps;

        RamOp? deferredRemovalRamOp = null;// = new RAMOp();

        foreach (var trxRamOp in Trx.RamOps)
        {
            if (trxRamOp.Namespace == RamOpNamespace.DEFERRED_TRX && trxRamOp.Action == RamOpAction.REMOVE)
            {
                deferredRemovalRamOp = trxRamOp;
                break;
            }
        }

        ResetTrx();
        Trx.RlimitOps = toRestoreRlimitOps;
        if (deferredRemovalRamOp != null)
        {
            Trx.RamOps = new List<RamOp>() { deferredRemovalRamOp };
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
    public void ReadStartBlock(string[] chunks)
    {
        if (chunks.Length != 1)
        {
            throw new Exception($"expected 1 fields, got {chunks.Length}");
        }

        var blockNum = Convert.ToInt64(chunks[0]);

        ResetBlock();
        ActiveBlockNum = blockNum;

        AbiDecoder.StartBlock(blockNum);
    }

    /// <summary>
    /// plugins/chain_plugin/chain_plugin.cpp:1162
    /// [ACCEPTED_BLOCK ${block_num} ${block_state_hex}]
    /// </summary>
    /// <param name="chunks"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    public Block ReadAcceptedBlock(string[] chunks) {
        if (chunks.Length != 2)
        {
            throw new Exception($"expected 2 fields, got {chunks.Length}");
        }

        var blockNum = Convert.ToInt64(chunks[0]);
        if(blockNum == ActiveBlockNum + 1)
        {
            ActiveBlockNum++;
        }

        if (ActiveBlockNum != blockNum)
        {
            Log.Information($"block_num {blockNum} doesn't match the active block num {ActiveBlockNum}");
        }

        var blockStateHex = chunks[1].HexStringToByteArray();

        var blockState = DeepMindDeserializer.DeepMindDeserializer.Deserialize<BlockState>(blockStateHex);

        var block = new Block()
        {
            Id = blockState.Id,
            Number = blockState.BlockNum,
            Version = 1,
            Header = blockState.Header,
            DposIrreversibleBlocknum = blockState.DPoSIrreversibleBlockNum,
            DposProposedIrreversibleBlocknum = blockState.DPoSProposedIrreversibleBlockNum,
            Validated = blockState.Validated,
            BlockrootMerkle = blockState.BlockrootMerkle,
            ProducerToLastProduced = blockState.ProducerToLastProduced,
            ProducerToLastImpliedIrb = blockState.ProducerToLastImpliedIrb,
            ActivatedProtocolFeatures = blockState.ActivatedProtocolFeatures,
            PendingSchedule = blockState.PendingSchedule,
            ActiveSchedule = blockState.ActiveSchedule,
            ValidBlockSigningAuthority = blockState.ValidBlockSigningAuthority,
            BlockSigningKey = blockState.ValidBlockSigningAuthority is BlockSigningAuthorityV0 blockSigningAuthority ? blockSigningAuthority.Keys.FirstOrDefault()?.Key ?? PublicKey.Empty : PublicKey.Empty,
            RlimitOps = Block.RlimitOps,
            UnfilteredImplicitTransactionOps = Block.UnfilteredImplicitTransactionOps,
            UnfilteredTransactionTraces = Block.UnfilteredTransactionTraces,
            ConfirmCount = new uint[blockState.ConfirmCount.Length],
            BlockExtensions = blockState.Block?.BlockExtensions ?? Array.Empty<KeyValuePair<ushort, char[]>>(),
            ProducerSignature = blockState.Block?.ProducerSignature ?? Signature.Empty,
            UnfilteredTransactions = blockState.Block?.Transactions.ToList() ?? new List<TransactionReceipt>(),

        };

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

        for (int i = 0; i < blockState.ConfirmCount.Length; i++)
        {
            block.ConfirmCount[i] = blockState.ConfirmCount[i];
        }

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

        for (int idx = 0; idx < block.UnfilteredTransactionTraces.Count; idx++)
        {
            var el = block.UnfilteredTransactionTraces[idx];
            el.Index = (ulong)idx;
            el.BlockTime = 0;// TODO block.Header.Timestamp;
            el.ProducerBlockId = block.Id;
            el.BlockNum = block.Number;

            //foreach (var actionTrace in el.ActionTraces)
            //{
	           // if (actionTrace.IsInput())
	           // {
		          //  block.UnfilteredExecutedInputActionCount++;
	           // }
            //}
        }

//	        zlog.Debug("blocking until abi decoder has decoded every transaction pushed to it")

        AbiDecoder.EndBlock(block);
        ResetBlock();
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

    private byte[][] ChecksumsToBytesSlices(byte[] merkleActiveNodes)
    {
        // TODO
        return new[] { merkleActiveNodes };
    }

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
    public void ReadAppliedTransaction(string[] chunks)
    {
        if (chunks.Length != 2)
        {
            throw new Exception($"expected 2 fields, got {chunks.Length}");
        }

        var blockNum = Convert.ToUInt32(chunks[0]);

        if (ActiveBlockNum != blockNum)
        {
//            throw new Exception($"saw transactions from block {blockNum} while active block is {ActiveBlockNum}");
            Log.Information($"saw transactions from block {blockNum} while active block is {ActiveBlockNum}");
        }

        var trxTrace = DeepMindDeserializer.DeepMindDeserializer.Deserialize<TransactionTrace>(chunks[1].HexStringToByteArray());

        RecordTransaction(trxTrace);
    }

    // Line formats:
    //  CREATION_OP ROOT ${action_id}
    //  CREATION_OP NOTIFY ${action_id}
    //  CREATION_OP INLINE ${action_id}
    //  CREATION_OP CFA_INLINE ${action_id}
    public void ReadCreationOp(string[] chunks)
    {
        if (chunks.Length != 2)
        {
            throw new Exception($"expected 2 fields, got {chunks.Length}");
        }

        var kind = chunks[0];
        if (kind != "ROOT" && kind != "NOTIFY" && kind != "INLINE" && kind != "CFA_INLINE")
        {
            throw new Exception($"kind must be one of ROOT, NOTIFY, CFA_INLINE or INLINE, got: {kind}");
        }

        var actionIndex = Convert.ToInt32(chunks[1]);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        RecordCreationOp(new CreationOp()
        {
            Kind = kind,
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
    public void ReadDbOp(string[] chunks)
    {
        if (chunks.Length != 8)
        {
            throw new Exception($"expected 8 fields, got {chunks.Length}");
        }

        var actionIndex = Convert.ToInt32(chunks[1]);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        var opString = chunks[0];

        var op = DbOpOperation.UNKNOWN;
        string oldData = string.Empty, newData = string.Empty;
        string oldPayer = string.Empty, newPayer = string.Empty;

        switch (opString)
        {
	        case "INS":
                op = DbOpOperation.INSERT;
                newData = chunks[7];
                newPayer = chunks[2];
				break;
            case "UPD":
                op = DbOpOperation.UPDATE;
                var dataChunks = chunks[7].Split(':');
	            if (dataChunks.Length != 2)
                {
                    throw new Exception("should have old and new data in field 8, found only one");
                }

                oldData = dataChunks[0];
                newData = dataChunks[1];

                var payerChunks = chunks[2].Split(':');
	            if (payerChunks.Length != 2)
                {
                    throw new Exception("should have two payers in field 3, separated by a ':', found only one");
                }

                oldPayer = payerChunks[0];
                newPayer = payerChunks[1];
				break;
            case "REM":
                op = DbOpOperation.REMOVE;
                oldData = chunks[7];
                oldPayer = chunks[2];
				break;
            default:
                throw new Exception($"unknown operation: {opString}");
        }

        byte[] oldBytes = new byte[] { }, newBytes = new byte[] { };
        if (!string.IsNullOrEmpty(oldData))
        {
            oldBytes = oldData.ToBytes();
	        /*if err != nil {
		        return fmt.Errorf("couldn't decode old_data: %s", err)
	        }*/
        }

        if (!string.IsNullOrEmpty(newData))
        {
            newBytes = newData.ToBytes();
	        /*if err != nil {
		        return fmt.Errorf("couldn't decode new_data: %s", err)
	        }*/
        }

        RecordDbOp(new DbOp()
        {
            Operation = op,
            ActionIndex = (uint) actionIndex,
            OldPayer = oldPayer,
            NewPayer = newPayer,
            Code = chunks[3],
            Scope = chunks[4],
            TableName = chunks[5],
            PrimaryKey = chunks[6],
            OldData = oldBytes,
            NewData = newBytes,
        });
    }

    // Line formats:
    //   DTRX_OP MODIFY_CANCEL ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP MODIFY_CREATE ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP CREATE        ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP CANCEL        ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    //   DTRX_OP PUSH_CREATE   ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
    public void ReadCreateOrCancelDTrxOp(string tag, string[] chunks)
    {
        if (chunks.Length != 11)
        {
            throw new Exception($"expected 11 fields, got {chunks.Length}");
        }

        var opString = chunks[1];
        var rawOp = Enum.Parse<DTrxOpOperation>(opString, true);//pbcodec.DTrxOp_Operation_value["OPERATION_" + opString]);

        var op = rawOp; //pbcodec.DTrxOp_Operation(rawOp);

        var actionIndex = Convert.ToInt32(chunks[2]);

        var trxHex = chunks[10].HexStringToByteArray();

        SignedTransaction signedTrx;// = new SignedTransaction();
        if (op == DTrxOpOperation.PUSH_CREATE)
        {
            signedTrx = DeepMindDeserializer.DeepMindDeserializer.Deserialize<SignedTransaction>(trxHex);
        }
        else
        {
            var trx = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Transaction>(trxHex);
            signedTrx = (SignedTransaction)trx;
        }

        RecordDTrxOp(new DTrxOp()
        {
            Operation = op,
            ActionIndex = (uint) actionIndex,
            Sender = chunks[3],
            SenderId = chunks[4],
            Payer = chunks[5],
            PublishedAt = chunks[6],
            DelayUntil = chunks[7],
            ExpirationAt = chunks[8],
            TransactionId = chunks[9],
            Transaction = signedTrx,
        });

        _ = Task.Run(() => AbiDecoder.ProcessSignedTransaction(signedTrx));
    }

    // Line format:
    //   DTRX_OP FAILED ${action_id}
    public void ReadFailedDTrxOp(string[] chunks)
    {
        if (chunks.Length != 3)
        {
            throw new Exception($"expected 3 fields, got {chunks.Length}");
        }

        var actionIndex = Convert.ToInt32(chunks[2]);
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
    public void ReadFeatureOpActivate(string[] chunks)
    {
        if (chunks.Length != 4)
        {
            throw new Exception($"expected 4 fields, got {chunks.Length}");
        }

        var feature = JsonSerializer.Deserialize<Feature>(chunks[3], _jsonSerializerOptions)!;
        // TODO does this work?
        //err:= json.Unmarshal(json.RawMessage(chunks[3]), &feature)
        /*if err != nil {
	        return fmt.Errorf("unmashall new feature data: %s", err)
        }*/

        RecordFeatureOp(new FeatureOp()
        {
            Kind = chunks[1],
            FeatureDigest = chunks[2],
            Feature = feature,
        });
    }

    // Line formats:
    //   FEATURE_OP PRE_ACTIVATE ${action_id} ${feature_digest} ${feature}
    public void ReadFeatureOpPreActivate(string[] chunks)
    {
        if (chunks.Length != 5)
        {
            throw new Exception($"expected 5 fields, got {chunks.Length}");
        }

        var actionIndex = Convert.ToInt32(chunks[2]);
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
            Kind = chunks[1],
            ActionIndex = (uint) actionIndex,
            FeatureDigest = chunks[3],
            Feature = feature,
        });
    }

    // Line formats: (the `[...]` represents optional fields)
    //   PERM_OP INS ${action_id} [${permission_id}] ${data}
    //   PERM_OP UPD ${action_id} [${permission_id}] ${data}
    //   PERM_OP REM ${action_id} [${permission_id}] ${data} <-- {"old": <old>, "new": <new>}
    public void ReadPermOp(string[] chunks)
    {
        var actionIndex = Convert.ToInt32(chunks[1]);

        var opString = chunks[0];
        var dataChunk = chunks[2];
        ulong permissionId = 0;

        // A `PERM_OP` with 5 fields have ["permission_id"] field in index #3 set and data chunk is actually index #4
        if (chunks.Length == 4)
        {
            permissionId = Convert.ToUInt64(chunks[2]);
            dataChunk = chunks[3];
        }

        var op = PermOpOperation.UNKNOWN;
        string oldData = string.Empty, newData = string.Empty;

        switch (opString) {
	        case "INS":
		        op = PermOpOperation.INSERT;
                //var feature = JsonSerializer.Deserialize<PermissionObject>(chunks[3]);
                newData = dataChunk;
                //                newData = Encoding.ASCII.GetBytes(dataChunk);
                break;
            case "UPD":
                op = PermOpOperation.UPDATE;
                //   var oldJSONResult = gjson.Get(dataChunk, "old")
                // TODO deserialize directly?
                JsonDocument jsonDocument = JsonDocument.Parse(dataChunk);
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

                oldData = oldJsonResult.GetRawText();
    	        newData = newJsonResult.GetRawText();
                break;
            case "REM":
                op = PermOpOperation.REMOVE;
                oldData = dataChunk;
                break;
            default:
                throw new Exception($"unknown PERM_OP op: {opString}");
        }

        var permOp = new PermOp()
        {
            Operation = op,
            ActionIndex = (uint) actionIndex,
        };

        if (newData.Length > 0)
        {
            var newPerm = JsonSerializer.Deserialize<PermissionObject>(newData, _jsonSerializerOptions)!;

            permOp.NewPerm = newPerm;
            permOp.NewPerm.Id = permissionId;
        }

        if (oldData.Length > 0)
        {
            var oldPerm = JsonSerializer.Deserialize<PermissionObject>(oldData, _jsonSerializerOptions)!;
	        /*err = json.Unmarshal(oldData, &oldPerm)
	        if err != nil {
		        return fmt.Errorf("unmashal old perm data: %s", err)
	        }*/
            permOp.OldPerm = oldPerm;
            permOp.OldPerm.Id = permissionId;
        }

        RecordPermOp(permOp);
    }

    // Line format:
    //   RAM_OP ${action_index} ${unique_key} ${namespace} ${action} ${legacy_tag} ${payer} ${new_usage} ${delta}
    public void ReadRamOp(string[] chunks)
    {
        if (chunks.Length != 8)
        {
            throw new Exception($"expected 8 fields, got {chunks.Length}");
        }

        var actionIndex = Convert.ToInt32(chunks[0]);

        var namespaceString = chunks[2];
        var @namespace = Enum.Parse<RamOpNamespace>(namespaceString, true);

        var actionString = chunks[3];
        var action = Enum.Parse<RamOpAction>(actionString, true);
        /*if !ok {
        return fmt.Errorf("action %q unknown", actionString)
        }*/

        var operationString = chunks[4];
        var operation = Enum.Parse<RamOpOperation>(operationString, true);
        /*if !ok {
        return fmt.Errorf("operation %q unknown", operationString)
        }*/

        var usage = Convert.ToUInt64(chunks[6]);
        /*if err != nil {
        return fmt.Errorf("usage is not a valid number, got: %q", chunks[4])
        }*/

        var delta = Convert.ToInt64(chunks[7]);
        /*if err != nil {
        return fmt.Errorf("delta is not a valid number, got: %q", chunks[5])
        }*/

        RecordRamOp(new RamOp()
        {
            ActionIndex = (uint) actionIndex,
            UniqueKey = chunks[1],
            Namespace = @namespace,
            Action = action,
            Operation = operation,
            Payer = chunks[5],
            Usage = usage,
            Delta = (long) delta,
        });
    }

    // Line format:
    //  Version 12
    //    DEEP_MIND_VERSION ${major_version}
    //
    //  Version 13
    //    DEEP_MIND_VERSION ${major_version} ${minor_version}
    public void ReadDeepmindVersion(string[] chunks)
    {
        var majorVersion = chunks[0];
        if (!InSupportedVersion(majorVersion)) {
            throw new Exception(
                $"deep mind reported version {majorVersion}, but this reader supports only {string.Join(", ", SupportedVersions)}");
        }
        Log.Information($"read deep mind version {majorVersion}");
    }

    public bool InSupportedVersion(string majorVersion)
    {
        foreach (var supportedVersion in SupportedVersions)
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
    public void ReadAbiStart(string[] chunks)
    {
        // TODO
        switch (chunks.Length) {
            case 2: // Version > 12 ?
                var blockNum = Convert.ToInt32(chunks[0]);
                var globalSequence = Convert.ToInt32(chunks[1]);
//                    logFields.Add(zap.Int("block_num", blockNum), zap.Int("global_sequence", globalSequence));
                break;
            default:
                throw new Exception($"expected to have either {{0}} or {{2}} fields, got {chunks.Length}");
            }

//	        zlog.Info("read ABI start marker", logFields...)
            AbiDecoder.ResetCache();
    }

    // Line format:
    //  Version 12
    //    ABIDUMP ABI ${block_num} ${contract} ${base64_abi}
    //
    //  Version 13
    //    ABIDUMP ABI ${contract} ${base64_abi}
    public void ReadAbiDump(string[] chunks)
    {
        string contract = string.Empty, rawAbi = string.Empty;
        switch (chunks.Length) {
            case 0: // Version < 14?
                break;
	        case 2: // Version 14?
                contract = chunks[0];
                rawAbi = chunks[1];
                break;
            default:
                throw new Exception($"expected to have either {{0}} or {{2}} fields, got {chunks.Length}");
        }

        if (TraceEnabled)
        {
            Log.Information($"read initial ABI for contract {contract}");
        }

        AbiDecoder.AddInitialAbi(contract, rawAbi);
    }

    // Line format:
    //   RAM_CORRECTION_OP ${action_id} ${correction_id} ${unique_key} ${payer} ${delta}
    public void ReadRamCorrectionOp(string[] chunks)
    {
        if (chunks.Length != 6)
        {
            throw new Exception($"expected 6 fields, got {chunks.Length}");
        }

        // We assume ${action_id} will always be 0, since called from onblock, so that's why we do not process it

        var delta = Convert.ToInt64(chunks[5]);
        /*if err != nil {
	        return fmt.Errorf("delta not a valid number, got: %q", chunks[5])
        }*/

        RecordRamCorrectionOp(new RamCorrectionOp()
        {
            CorrectionId = chunks[2],
            UniqueKey = chunks[3],
            Payer = chunks[4],
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
    public void ReadRlimitOp(string[] chunks)
    {
        if (chunks.Length != 3)
        {
            throw new Exception($"expected 3 fields, got {chunks.Length}");
        }

        var kindString = chunks[0];
        var operationString = chunks[1];

        var operation = RlimitOpOperation.UNKNOWN;
        switch (operationString) {
	        case "INS":
                operation = RlimitOpOperation.INSERT;
                break;
            case "UPD":
                operation = RlimitOpOperation.UPDATE;
                break;
            default:
                throw new Exception($"operation {operationString} is unknown");
        }

        RlimitOp op;
        var data = chunks[2];

        switch (kindString) {
	        case "CONFIG":
                op = JsonSerializer.Deserialize<RlimitConfig>(data, _jsonSerializerOptions)!;
                break;
            case "STATE":
                op = JsonSerializer.Deserialize<RlimitState>(data, _jsonSerializerOptions)!;
                break;
            case "ACCOUNT_LIMITS":
                op = JsonSerializer.Deserialize<RlimitAccountLimits>(data, _jsonSerializerOptions)!;
                break;
            case "ACCOUNT_USAGE":
                op = JsonSerializer.Deserialize<RlimitAccountUsage>(data, _jsonSerializerOptions)!;
                break;
            default:
                throw new Exception($"unknown kind: {kindString}");
        }

        op.Operation = operation;

        RecordRlimitOp(op);
    }

    // Line formats:
    //   TBL_OP INS ${action_id} ${code} ${scope} ${table} ${payer}
    //   TBL_OP REM ${action_id} ${code} ${scope} ${table} ${payer}
    public void ReadTableOp(string[] chunks)
    {
        if (chunks.Length != 6)
        {
            throw new Exception($"expected 6 fields, got {chunks.Length}");
        }

        var actionIndex = Convert.ToInt32(chunks[1]);

        var opString = chunks[0];
        var op = TableOpOperation.UNKNOWN;
        switch (opString) {
	        case "INS":
                op = TableOpOperation.INSERT;
                break;
            case "REM":
                op = TableOpOperation.REMOVE;
                break;
            default:
                throw new Exception($"unknown kind: {opString}");
        }

        RecordTableOp(new TableOp()
        {
            Operation = op,
            ActionIndex = (uint) actionIndex,
            Payer = chunks[5],
            Code = chunks[2],
            Scope = chunks[3],
            TableName = chunks[4],
        });
    }

    // Line formats:
    //   TRX_OP CREATE onblock|onerror ${id} ${trx}
    public void ReadTrxOp(string[] chunks)
    {
        if (chunks.Length != 4)
        {
            throw new Exception($"expected 4 fields, got {chunks.Length}");
        }

        var opString = chunks[0];
        var op = TrxOpOperation.UNKNOWN;
        switch (opString) {
	        case "CREATE":
                op = TrxOpOperation.CREATE;
                break;
            default:
                throw new Exception($"unknown kind: {opString}");
        }

        var name = chunks[1];
        var trxId = chunks[2];

        var trxHex = chunks[3].HexStringToByteArray();

        var trx = DeepMindDeserializer.DeepMindDeserializer.Deserialize<SignedTransaction>(trxHex);

        RecordTrxOp(new TrxOp()
        {
            Operation = op,
            Name = name, // "onblock" or "onerror"
            TransactionId = trxId, // the hash of the transaction
            Transaction = trx,
        });
    }
}