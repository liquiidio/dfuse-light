using System.Text;
using System.Text.Json;
using DeepReader.Types;
using DeepReader.Types.Enums;
using DeepReader.Deserializer;

namespace DeepReader.Classes;

public class ParseCtx
{
    public Block Block;

    public long ActiveBlockNum;

    public TransactionTrace Trx;

    public List<CreationOp> CreationOps;

    public bool TraceEnabled;

    public string[] SupportedVersions = new[] {"14"};

    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

//        public conversionOptions[] conversionOption;*/

    public ParseCtx()
    {
        Block = new Block();
        ActiveBlockNum = 0;
        Trx = new TransactionTrace();
        CreationOps = new List<CreationOp>();
        TraceEnabled = false;
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
//            creationOps = null;
    }

	public void RecordCreationOp(CreationOp operation)
    {
        CreationOps.Add(operation);
    }

	public void RecordDbOp(DBOp operation)
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

    public void RecordRamOp(RAMOp operation)
    {
        Trx.RamOps.Add(operation);
    }

    public void RecordRamCorrectionOp(RAMCorrectionOp operation)
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
            Trx.RamOps = TransferDeferredRemovedRAMOp(Trx.RamOps, failedTrace);

	        // The only possibilty to have failed deferred trace, is when the deferred execution
	        // resulted in a subjetive failure, which is really a soft fail. So, when the receipt is
	        // not set, let's re-create it here with soft fail status only.
	        if (failedTrace.Receipt == null)
            {
                failedTrace.Receipt = new TransactionReceiptHeader()
                {
                    Status = TransactionStatus.SOFTFAIL
                };
            }

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
            if (trace.Receipt == null || trace.Receipt.Status == TransactionStatus.HARDFAIL)
            {
                RevertOpsDueToFailedTransaction();
            }
        }

        // All this stiching of ops into trace must be performed after `if` because the if can revert them all
        var creationTreeRoots = CreationTree.ComputeCreationTree(CreationOps);
        /* TODO if err != nil {
            return fmt.Errorf("compute creation tree: %s", err)
        }*/

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

        AbiDecoder.ProcessTransaction(trace);

        ResetTrx();
    }

    private IList<RAMOp> TransferDeferredRemovedRAMOp(ICollection<RAMOp> initialRAMOps, TransactionTrace target)
    {
        IList<RAMOp> filteredRAMOps = new List<RAMOp>();
        foreach (var ramOp in initialRAMOps)
        {
            if (ramOp.Namespace == RAMOp_Namespace.DEFERRED_TRX && ramOp.Action == RAMOpAction.REMOVE)
            {
                target.RamOps.Add(ramOp);
            } else
            {
                filteredRAMOps.Add(ramOp);
            }            
        }

        return filteredRAMOps;
    }

    public void RevertOpsDueToFailedTransaction() 
    {
        // We must keep the deferred removal, as this RAM changed is **not** reverted by nodeos, unlike all other ops
        // as well as the RLimitOps, which happens at a location that does not revert.
        var toRestoreRlimitOps = Trx.RlimitOps;

        RAMOp? deferredRemovalRAMOp = null;// = new RAMOp();

        foreach (var trxRamOp in Trx.RamOps)
        {
            if (trxRamOp.Namespace == RAMOp_Namespace.DEFERRED_TRX && trxRamOp.Action == RAMOpAction.REMOVE)
            {
                deferredRemovalRAMOp = trxRamOp;
                break;
            }
        }

        ResetTrx();
        Trx.RlimitOps = toRestoreRlimitOps;
        if (deferredRemovalRAMOp != null)
        {
            Trx.RamOps = new List<RAMOp>() { deferredRemovalRAMOp };
        }
    }

    public RAMOp[] TransferDeferredRemovedRamOp(RAMOp[] initialRAMOps, TransactionTrace target)
    {
        List<RAMOp> filteredRAMOps = new List<RAMOp>();
        foreach (var initialRamOp in initialRAMOps)
        {
			if (initialRamOp.Namespace == RAMOp_Namespace.DEFERRED_TRX && initialRamOp.Action == RAMOpAction.REMOVE)
            {
                target.RamOps.Add(initialRamOp);
            }
            else
            {
                filteredRAMOps.Add(initialRamOp);
            }
		}
        return filteredRAMOps.ToArray();
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

        // TODO
        AbiDecoder.StartBlock(blockNum);
        /*if err := ctx.abiDecoder.startBlock(uint64(blockNum)); err != nil {
	        return fmt.Errorf("abi decoder: %w", err)
        }*/
    }

	// Line format:
	//   ACCEPTED_BLOCK ${block_num} ${block_state_hex}
    public Block ReadAcceptedBlock(string[] chunks) {
        if (chunks.Length != 2)
        {
                throw new Exception($"expected 2 fields, got {chunks.Length}");
        }

        var blockNum = Convert.ToInt64(chunks[0]);
        /*if err != nil {
	        return null;, fmt.Errorf("block_num not a valid string, got: %q", chunks[1])
        }*/

        if (ActiveBlockNum != blockNum)
        {
            Console.WriteLine($"block_num {blockNum} doesn't match the active block num {ActiveBlockNum}");
//            throw new Exception($"block_num {blockNum} doesn't match the active block num {ActiveBlockNum}");
        }

        var blockStateHex = chunks[1].HexStringToByteArray();
        /*
        if err != nil {
	        return null;, fmt.Errorf("unable to decode block %d state hex: %w", blockNum, err)
        }
        */

        var blockState = Deserializer.Deserializer.Deserialize<BlockState>(blockStateHex);

        /*if err != nil {
	        return null;, fmt.Errorf("unmarshalling binary block state: %w", err)
        }*/

        var signedBlock = blockState.SignedBlock;

        var block = new Block();

        // this is hydrator.hydrateblock ... 
        
        block.Id = Encoding.ASCII.GetString(blockState.BlockID);
        block.Number = blockState.BlockNum;
        // Version 1: Added the total counts (ExecutedInputActionCount, ExecutedTotalActionCount,
        // TransactionCount, TransactionTraceCount)
        block.Version = 1;
        block.Header = BlockHeaderToDEOS(signedBlock);
        block.BlockExtensions = ExtensionsToDEOS(signedBlock.BlockExtensions);
        block.DposIrreversibleBlocknum = blockState.DPoSIrreversibleBlockNum;
        block.DposProposedIrreversibleBlocknum = blockState.DPoSProposedIrreversibleBlockNum;
        block.Validated = blockState.Validated;
        block.BlockrootMerkle = BlockrootMerkleToDEOS(blockState.BlockrootMerkle);
        block.ProducerToLastProduced = ProducerToLastProducedToDEOS(blockState.ProducerToLastProduced);
        block.ProducerToLastImpliedIrb = ProducerToLastImpliedIrbToDEOS(blockState.ProducerToLastImpliedIRB);
        block.ActivatedProtocolFeatures = ActivatedProtocolFeaturesToDEOS(blockState.ActivatedProtocolFeatures);
        block.ProducerSignature = Encoding.ASCII.GetString(signedBlock.ProducerSignature, 0, signedBlock.ProducerSignature.Length);

        block.ConfirmCount = new uint[blockState.ConfirmCount.Length];
        for (int i = 0; i < blockState.ConfirmCount.Length; i++)
        {
            block.ConfirmCount[i] = blockState.ConfirmCount[i];
        }

        if (blockState.PendingSchedule != null)
        {
            block.PendingSchedule = PendingScheduleToDEOS(blockState.PendingSchedule);
        }

        /// Specific versions handling

//        var blockSigningKey = blockState.BlockSigningKeyV1; 
        var schedule = blockState.ActiveSchedule;
        var signingAuthority = blockState.ValidBlockSigningAuthorityV2;

        // Only in EOSIO 1.x
        //if (blockSigningKey != null)
        //{
        //    block.BlockSigningKey = Encoding.ASCII.GetString(blockSigningKey);
        //}

        if (schedule.V1 != null)
        {
            block.ActiveScheduleV1 = schedule.V1;
        }

        // Only in EOSIO 2.x
        if (signingAuthority != null)
        {
            block.ValidBlockSigningAuthorityV2 = signingAuthority;
        }

        if (schedule.V2 != null)
        {
            block.ActiveScheduleV2 = schedule.V2;
        }

        // End (versions)

        block.UnfilteredTransactionCount = (uint)signedBlock.Transactions.Length;
        block.UnfilteredTransactions = signedBlock.Transactions;

        block.UnfilteredTransactionTraceCount = (uint) block.UnfilteredTransactionTraces.Count;

        for (int idx = 0; idx < block.UnfilteredTransactionTraceCount; idx++)
        {
            var el = block.UnfilteredTransactionTraces[idx];
            el.Index = (ulong)idx;
            el.BlockTime = 0;// TODO block.Header.Timestamp;
            el.ProducerBlockId = block.Id;
            el.BlockNum = block.Number;

            foreach (var actionTrace in el.ActionTraces)
            {
	            if (actionTrace.IsInput())
	            {
		            block.UnfilteredExecutedInputActionCount++;
	            }
            }
        }

//	        zlog.Debug("blocking until abi decoder has decoded every transaction pushed to it")

// TODO
        AbiDecoder.EndBlock(block);
        /*if err != nil {
	        return null;, fmt.Errorf("abi decoding post-process failed: %w", err)
        }*/

//	        zlog.Debug("abi decoder terminated all decoding operations, resetting block")
        ResetBlock();
        return block;
    }

    private PendingProducerSchedule PendingScheduleToDEOS(PendingSchedule blockStatePendingSchedule)
    {
        var pendingProducerSchedule = new PendingProducerSchedule()
        {
	        ScheduleLibNum = blockStatePendingSchedule.ScheduleLIBNum,
	        ScheduleHash = blockStatePendingSchedule.ScheduleHash,
        };

        /// Specific versions handling

        // Only in EOSIO 1.x
        if (blockStatePendingSchedule.Schedule.V1 != null)
        {
	        pendingProducerSchedule.ScheduleV1 = blockStatePendingSchedule.Schedule.V1;
        }

        // Only in EOSIO 2.x
        if (blockStatePendingSchedule.Schedule.V2 != null)
        {
	        pendingProducerSchedule.ScheduleV2 = blockStatePendingSchedule.Schedule.V2;
        }

        // End (versions)
        return pendingProducerSchedule;
    }

    private ActivatedProtocolFeatures ActivatedProtocolFeaturesToDEOS(ProtocolFeatureActivationSet blockStateActivatedProtocolFeatures)
    {

        return new ActivatedProtocolFeatures()
        {
            // TODO !! ProtocolFeatures multi-dim array?
            ProtocolFeatures = blockStateActivatedProtocolFeatures.ProtocolFeatures.Length > 0 ? checksumsToBytesSlices(blockStateActivatedProtocolFeatures.ProtocolFeatures[0]) : Array.Empty<byte[]>()
        };
    }

    private ProducerToLastImpliedIRB[] ProducerToLastImpliedIrbToDEOS(PairAccountNameBlockNum[] blockStateProducerToLastImpliedIrb)
    {
        var producerToLastImpliedIRB = new ProducerToLastImpliedIRB[blockStateProducerToLastImpliedIrb.Length];
        for (int i = 0; i < blockStateProducerToLastImpliedIrb.Length; i++)
        {
	        producerToLastImpliedIRB[i] = new ProducerToLastImpliedIRB()
	        {
		        Name = blockStateProducerToLastImpliedIrb[i].AccountName,
		        LastBlockNumProduced = blockStateProducerToLastImpliedIrb[i].BlockNum
	        };
        }
        return producerToLastImpliedIRB;
    }

    private ProducerToLastProduced[] ProducerToLastProducedToDEOS(PairAccountNameBlockNum[] blockStateProducerToLastProduced)
    {
        var producerToLastProduced = new ProducerToLastProduced[blockStateProducerToLastProduced.Length];
        for (int i = 0; i < blockStateProducerToLastProduced.Length; i++)
        {
	        producerToLastProduced[i] = new ProducerToLastProduced()
	        {
		        Name = blockStateProducerToLastProduced[i].AccountName,
		        LastBlockNumProduced = blockStateProducerToLastProduced[i].BlockNum
	        };
        }

        return producerToLastProduced;
    }

    private BlockRootMerkle BlockrootMerkleToDEOS(MerkleRoot merkle)
    {
        return new BlockRootMerkle()
        {
	        NodeCount = (uint)merkle.NodeCount,
	        ActiveNodes = checksumsToBytesSlices(merkle.ActiveNodes)
        };
    }

    private byte[][] checksumsToBytesSlices(byte[] merkleActiveNodes)
    {
        // TODO
        return new[] { merkleActiveNodes };
    }

    private Extension[] ExtensionsToDEOS(Extension[] signedBlockBlockExtensions)
    {
        // ?! TODO
        return signedBlockBlockExtensions;
    }

    private BlockHeader BlockHeaderToDEOS(SignedBlock signedBlock)
    {
        var blockHeader = new BlockHeader()
        {
	        Timestamp = signedBlock.Timestamp,
	        Producer = signedBlock.Producer,
	        Confirmed = signedBlock.Confirmed,
	        Previous = signedBlock.Previous,
	        TransactionMroot = signedBlock.TransactionMroot,
	        ActionMroot = signedBlock.ActionMroot,
	        ScheduleVersion = signedBlock.ScheduleVersion,
	        HeaderExtensions = ExtensionsToDEOS(signedBlock.HeaderExtensions),
        };

        if (blockHeader.NewProducersV1 != null)
        {
	        blockHeader.NewProducersV1 = blockHeader.NewProducersV1;
        }
        
        return blockHeader;
    }

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
            Console.WriteLine($"saw transactions from block {blockNum} while active block is {ActiveBlockNum}");
        }

        // TODO unmarshal?
        var trxTrace = Deserializer.Deserializer.Deserialize<TransactionTrace>(chunks[1].HexStringToByteArray());

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
        if (chunks.Length != 9)
        {
            throw new Exception($"expected 9 fields, got {chunks.Length}");
        }

        var actionIndex = Convert.ToInt32(chunks[2]);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        var opString = chunks[1];

        var op = DBOpOperation.UNKNOWN;
        string oldData = string.Empty, newData = string.Empty;
        string oldPayer = string.Empty, newPayer = string.Empty;

        switch (opString)
        {
	        case "INS":
                op = DBOpOperation.INSERT;
                newData = chunks[8];
                newPayer = chunks[3];
				break;
            case "UPD":
                op = DBOpOperation.UPDATE;
                var dataChunks = chunks[8].Split(':');
	            if (dataChunks.Length != 2)
                {
                    throw new Exception("should have old and new data in field 8, found only one");
                }

                oldData = dataChunks[0];
                newData = dataChunks[1];

                var payerChunks = chunks[3].Split(':');
	            if (payerChunks.Length != 2)
                {
                    throw new Exception("should have two payers in field 3, separated by a ':', found only one");
                }

                oldPayer = payerChunks[0];
                newPayer = payerChunks[1];
				break;
            case "REM":
                op = DBOpOperation.REMOVE;
                oldData = chunks[8];
                oldPayer = chunks[3];
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

        RecordDbOp(new DBOp()
        {
            Operation = op,
            ActionIndex = (uint) actionIndex,
            OldPayer = oldPayer,
            NewPayer = newPayer,
            Code = chunks[4],
            Scope = chunks[5],
            TableName = chunks[6],
            PrimaryKey = chunks[7],
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
        /*if !ok {
	        return fmt.Errorf("operation %q unknown", opString)
        }*/

        var op = rawOp; //pbcodec.DTrxOp_Operation(rawOp);

        var actionIndex = Convert.ToInt32(chunks[2]);
        /*
        if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }
        */

        var trxHex = chunks[10].HexStringToByteArray();
        /*if err != nil {
	        return fmt.Errorf("unable to decode signed transaction hex: %w", err)
        }*/

        SignedTransaction signedTrx;// = new SignedTransaction();
        if (op == DTrxOpOperation.PUSH_CREATE)
        {
            // TODO unmarshal
            signedTrx = Deserializer.Deserializer.Deserialize<SignedTransaction>(trxHex);
            /*if err != nil {
		        return fmt.Errorf("unmarshal binary signed transaction: %w", err)
	        }*/
        }
        else
        {
            // TODO unmarshal
            var trx = Deserializer.Deserializer.Deserialize<Transaction>(trxHex);
            /*
	        if err != nil {
		        return fmt.Errorf("unmarshal binary transaction: %w", err)
	        }
	        */
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

        var feature = JsonSerializer.Deserialize<Feature>(chunks[3], jsonSerializerOptions);
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

        var feature = JsonSerializer.Deserialize<Feature>(chunks[4], jsonSerializerOptions);
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
        // TODO
        /*chunks, err:= splitNToM(line, 4, 5)
        if err != nil {
	        return err
        }*/

        var actionIndex = Convert.ToInt32(chunks[1]);
        /*if err != nil {
	        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
        }*/

        var opString = chunks[0];
        var dataChunk = chunks[2];
        ulong permissionID = 0;

        // A `PERM_OP` with 5 fields have ["permission_id"] field in index #3 set and data chunk is actually index #4
        if (chunks.Length == 4)
        {
            permissionID = Convert.ToUInt64(chunks[2]);
	        /*if err != nil {
		        return fmt.Errorf("permission_id is not a valid number, got: %q", chunks[3])
	        }*/
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
            var newPerm = JsonSerializer.Deserialize<PermissionObject>(newData, jsonSerializerOptions);
	        /*
	        err = json.Unmarshal(newData, &newPerm)
	        if err != nil {
		        return fmt.Errorf("unmashal new perm data: %s", err)
	        }
	        */

            permOp.NewPerm = newPerm;
            permOp.NewPerm.Id = permissionID;
        }

        if (oldData.Length > 0)
        {
            var oldPerm = JsonSerializer.Deserialize<PermissionObject>(oldData, jsonSerializerOptions);
	        /*err = json.Unmarshal(oldData, &oldPerm)
	        if err != nil {
		        return fmt.Errorf("unmashal old perm data: %s", err)
	        }*/
            permOp.OldPerm = oldPerm;
            permOp.OldPerm.Id = permissionID;
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
        var @namespace = Enum.Parse<RAMOp_Namespace>(namespaceString, true);

        var actionString = chunks[3];
        var action = Enum.Parse<RAMOpAction>(actionString, true);
        /*if !ok {
        return fmt.Errorf("action %q unknown", actionString)
        }*/

        var operationString = chunks[4];
        var operation = Enum.Parse<RAMOp_Operation>(operationString, true);
        /*if !ok {
        return fmt.Errorf("operation %q unknown", operationString)
        }*/

        var usage = Convert.ToUInt64(chunks[6]);
        /*if err != nil {
        return fmt.Errorf("usage is not a valid number, got: %q", chunks[4])
        }*/

        var delta = Convert.ToUInt64(chunks[7]);
        /*if err != nil {
        return fmt.Errorf("delta is not a valid number, got: %q", chunks[5])
        }*/

        RecordRamOp(new RAMOp()
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
        if (!inSupportedVersion(majorVersion)) {
            throw new Exception(
                $"deep mind reported version {majorVersion}, but this reader supports only {string.Join(", ", SupportedVersions)}");
        }

//	        zlog.Info("read deep mind version", zap.String("major_version", majorVersion))
    }

    public bool inSupportedVersion(string majorVersion)
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
        switch (chunks.Length) {
	        case 2: // Version 12
                break;
            case 4: // Version 13
                var blockNum = Convert.ToInt32(chunks[2]);
	            /*if err != nil {
			            return fmt.Errorf("block_num is not a valid number, got: %q", chunks[2])
	            }*/

                var globalSequence = Convert.ToInt32(chunks[3]);
	            /*if err != nil {
			            return fmt.Errorf("global_sequence_num is not a valid number, got: %q", chunks[3])
	            }*/

//                    logFields.Add(zap.Int("block_num", blockNum), zap.Int("global_sequence", globalSequence));
                break;
            default:
                throw new Exception($"expected to have either {{2}} or {{4}} fields, got {chunks.Length}");
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
        string contract = string.Empty, rawABI = string.Empty;
        switch (chunks.Length) {
	        case 5: // Version 12
                contract = chunks[3];
                rawABI = chunks[4];
                break;
            case 4: // Version 13
                contract = chunks[2];
                rawABI = chunks[3];
                break;
        }

        if (TraceEnabled) {
//		        zlog.Debug("read initial ABI for contract", zap.String("contract", contract))
        }

        AbiDecoder.AddInitialABI(contract, rawABI);
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

        RecordRamCorrectionOp(new RAMCorrectionOp()
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
                op = JsonSerializer.Deserialize<RlimitConfig>(data, jsonSerializerOptions);
                break;
            case "STATE":
                op = JsonSerializer.Deserialize<RlimitState>(data, jsonSerializerOptions);
                break;
            case "ACCOUNT_LIMITS":
                op = JsonSerializer.Deserialize<RlimitAccountLimits>(data, jsonSerializerOptions);
                break;
            case "ACCOUNT_USAGE":
                op = JsonSerializer.Deserialize<RlimitAccountUsage>(data, jsonSerializerOptions);
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
        if (chunks.Length != 7)
        {
            throw new Exception($"expected 7 fields, got {chunks.Length}");
        }

        var actionIndex = Convert.ToInt32(chunks[2]);

        var opString = chunks[1];
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
            Payer = chunks[6],
            Code = chunks[3],
            Scope = chunks[4],
            TableName = chunks[5],
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
        var trxID = chunks[2];

        var trxHex = chunks[3].HexStringToByteArray();

        var trx = Deserializer.Deserializer.Deserialize<SignedTransaction>(trxHex);

        RecordTrxOp(new TrxOp()
        {
            Operation = op,
            Name = name, // "onblock" or "onerror"
            TransactionId = trxID, // the hash of the transaction
            Transaction = trx,
        });
    }
}