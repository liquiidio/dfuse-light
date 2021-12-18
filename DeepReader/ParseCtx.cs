namespace DeepReader;

public class ParseCtx
{
        public Block block;

        public long ActiveBlockNum;

        public TransactionTrace trx;

        public List<CreationOp> creationOps;

//        public conversionOptions[] conversionOption;*/

        public ParseCtx()
        {

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
			
            block = new Block();
        }

		public void ResetTrx()
        {
            trx = new TransactionTrace();
//            creationOps = null;
        }

		public void RecordCreationOp(CreationOp operation)
        {
            creationOps.Add(operation);
        }

		public void RecordDbOp(DBOp operation)
        {
            trx.DbOps.Add(operation);
        }

		public void RecordDTrxOp(DTrxOp transaction)
        {
            trx.DtrxOps.Add(transaction);
	   
	        if (transaction.Operation == DTrxOp.Types.Operation.Failed)
            {
                RevertOpsDueToFailedTransaction();
            }
		}

        public void RecordFeatureOp(FeatureOp operation)
        {
            trx.FeatureOps.Add(operation);
        }

        public void RecordPermOp(PermOp operation)
        {
            trx.PermOps.Add(operation);
        }

        public void RecordRamOp(RAMOp operation)
        {
            trx.RamOps.Add(operation);
        }

        public void RecordRamCorrectionOp(RAMCorrectionOp operation)
        {
            trx.RamCorrectionOps.Add(operation);
        }

        public void RecordRlimitOp(RlimitOp operation)
		{
			if (operation.KindCase == RlimitOp.KindOneofCase.Config || operation.KindCase == RlimitOp.KindOneofCase.State)
            {
                block.RlimitOps.Add(operation);
            }
			else if (operation.KindCase == RlimitOp.KindOneofCase.AccountLimits || operation.KindCase == RlimitOp.KindOneofCase.AccountUsage) {
				trx.RlimitOps.Add(operation);
	  	    }
		}

        public void RecordTableOp(TableOp operation)
        {
            trx.TableOps.Add(operation);
        }

        public void RecordTrxOp(TrxOp operation)
        {
            block.UnfilteredImplicitTransactionOps.Add(operation);
        }

        public Error RecordTransaction(TransactionTrace trace)
        {
            var failedTrace = trace.FailedDtrxTrace;
	        if (failedTrace != null) {
		        // Having a `FailedDtrxTrace` means the `trace` we got is an `onerror` handler.
		        // In this block, we perform all the logic to correctly record the `onerror`
		        // handler trace and the actual deferred transaction trace that failed.

		        // The deferred transaction removal RAM op needs to be attached to the failed trace, not the onerror handler
                trx.RamOps = TransferDeferredRemovedRAMOp(trx.RamOps, failedTrace);

		        // The only possibilty to have failed deferred trace, is when the deferred execution
		        // resulted in a subjetive failure, which is really a soft fail. So, when the receipt is
		        // not set, let's re-create it here with soft fail status only.
		        if (failedTrace.Receipt == null)
                {
                    failedTrace.Receipt = new TransactionReceiptHeader()
                    {
                        Status = TransactionStatus.Softfail
                    };
                }

                // We add the failed deferred trace first, before the "real" trace (the `onerror` handler)
                // since it was ultimetaly ran first. There is no ops possible on the trace expect the
                // transferred RAM op, so it's all good to attach it directly.
                block.UnfilteredTransactionTraces.Add(failedTrace);

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
                if (trace.Receipt == null || trace.Receipt.Status == TransactionStatus.Hardfail)
                {
                    RevertOpsDueToFailedTransaction();
                }
	        }

	        // All this stiching of ops into trace must be performed after `if` because the if can revert them all
            var creationTreeRoots = CreationTree.ComputeCreationTree(creationOps);
	        /* TODO if err != nil {
    	        return fmt.Errorf("compute creation tree: %s", err)
	        }*/

            trace.CreationTree = EosToProto.CreationTreeToDEOS(CreationTree.toFlatTree(creationTreeRoots));
            trace.DtrxOps = trx.DtrxOps;
            trace.DbOps = trx.DbOps;
            trace.FeatureOps = trx.FeatureOps;
            trace.PermOps = trx.PermOps;
            trace.RamOps = trx.RamOps;
            trace.RamCorrectionOps = trx.RamCorrectionOps;
            trace.RlimitOps = trx.RlimitOps;
            trace.TableOps = trx.TableOps;

	        block.UnfilteredTransactionTraces.Add(trace);

	        abiDecoder.processTransaction(trace);

            ResetTrx();
	        return null;
        }

        public void RevertOpsDueToFailedTransaction() 
        {
            // We must keep the deferred removal, as this RAM changed is **not** reverted by nodeos, unlike all other ops
            // as well as the RLimitOps, which happens at a location that does not revert.
            var toRestoreRlimitOps = trx.RlimitOps;

            RAMOp deferredRemovalRAMOp = null;// = new RAMOp();

            foreach (var trxRamOp in trx.RamOps)
            {
                if (trxRamOp.Namespace == RAMOp.Types.Namespace.DeferredTrx && trxRamOp.Action == RAMOp.Types.Action.Remove)
                {
                    deferredRemovalRAMOp = trxRamOp;
                    break;
                }
            }

            ResetTrx();
            trx.RlimitOps = toRestoreRlimitOps;
	        if (deferredRemovalRAMOp != null)
            {
                trx.RamOps = new RepeatedField<RAMOp>() {deferredRemovalRAMOp};
            }
        }

        public RAMOp[] TransferDeferredRemovedRamOp(RAMOp[] initialRAMOps, TransactionTrace target)
        {
            List<RAMOp> filteredRAMOps = new List<RAMOp>();
            foreach (var initialRamOp in initialRAMOps)
            {
				if (initialRamOp.Namespace == RAMOp.Types.Namespace.DeferredTrx && initialRamOp.Action == RAMOp.Types.Action.Remove)
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
        public Error ReadStartBlock(string[] chunks)
        {
	        if (chunks.Length != 2)
            {
                return new Error($"expected 2 fields, got {chunks.Length}");
            }

            var blockNum = Convert.ToInt64(chunks[1]);

            ResetBlock();
            ActiveBlockNum = blockNum;

            // TODO
            abiDecoder.startBlock(blockNum);
            /*if err := ctx.abiDecoder.startBlock(uint64(blockNum)); err != nil {
		        return fmt.Errorf("abi decoder: %w", err)
	        }*/

            return null;
        }

		// Line format:
		//   ACCEPTED_BLOCK ${block_num} ${block_state_hex}
        public Block ReadAcceptedBlock(string[] chunks) {
            if (chunks.Length != 3)
            {
//                throw new Error($"expected 3 fields, got {chunks.Length}");
            }

            var blockNum = Convert.ToInt64(chunks[1]);
	        /*if err != nil {
		        return null;, fmt.Errorf("block_num not a valid string, got: %q", chunks[1])
	        }*/

	        if (ActiveBlockNum != blockNum)
            {
                throw new Error($"block_num {blockNum} doesn't match the active block num {ActiveBlockNum}");
            }

            var blockStateHex = chunks[2].ToBytes();
	        /*
	        if err != nil {
		        return null;, fmt.Errorf("unable to decode block %d state hex: %w", blockNum, err)
	        }
	        */

            var blockState = Deserializer.Deserialize<BlockState>(blockStateHex);

	        /*if err != nil {
		        return null;, fmt.Errorf("unmarshalling binary block state: %w", err)
	        }*/

            var signedBlock = blockState.SignedBlock;

            var block = new Block();

            block.Id = blockState.BlockID.ToString();
            block.Number = blockState.BlockNum;
	        // Version 1: Added the total counts (ExecutedInputActionCount, ExecutedTotalActionCount,
	        // TransactionCount, TransactionTraceCount)
            block.Version = 1;
            block.Header = BlockHeaderToDEOS(&signedBlock.BlockHeader);
            block.BlockExtensions = ExtensionsToDEOS(signedBlock.BlockExtensions);
            block.DposIrreversibleBlocknum = blockState.DPoSIrreversibleBlockNum;
            block.DposProposedIrreversibleBlocknum = blockState.DPoSProposedIrreversibleBlockNum;
            block.Validated = blockState.Validated;
            block.BlockrootMerkle = BlockrootMerkleToDEOS(blockState.BlockrootMerkle);
            block.ProducerToLastProduced = ProducerToLastProducedToDEOS(blockState.ProducerToLastProduced);
            block.ProducerToLastImpliedIrb = ProducerToLastImpliedIrbToDEOS(blockState.ProducerToLastImpliedIRB);
            block.ActivatedProtocolFeatures = ActivatedProtocolFeaturesToDEOS(blockState.ActivatedProtocolFeatures);
            block.ProducerSignature = signedBlock.ProducerSignature.String();

	        block.ConfirmCount = make([]uint32, len(blockState.ConfirmCount))
	        for i, count := range blockState.ConfirmCount {
		        ctx.block.ConfirmCount[i] = uint32(count)
	        }

	        if (blockState.PendingSchedule != null)
            {
                block.PendingSchedule = PendingScheduleToDEOS(blockState.PendingSchedule);
            }

            /// Specific versions handling

            var blockSigningKey = blockState.BlockSigningKeyV1; 
            var schedule = blockState.ActiveSchedule;
            var signingAuthority = blockState.ValidBlockSigningAuthorityV2;

	        // Only in EOSIO 1.x
	        if (blockSigningKey != null)
            {
                block.BlockSigningKey = blockSigningKey.String();
            }

	        if (schedule.V1 != null)
            {
                block.ActiveScheduleV1 = ProducerScheduleToDEOS(schedule.V1);
            }

	        // Only in EOSIO 2.x
	        if (signingAuthority != null)
            {
                block.ValidBlockSigningAuthorityV2 = BlockSigningAuthorityToDEOS(signingAuthority);
            }

	        if (schedule.V2 != null)
            {
                block.ActiveScheduleV2 = ProducerAuthorityScheduleToDEOS(schedule.V2);
            }

	        // End (versions)

	        block.UnfilteredTransactionCount = uint32(len(signedBlock.Transactions))
	        for idx, transaction := range signedBlock.Transactions {
	        deosTransaction:= TransactionReceiptToDEOS(&transaction)
		        deosTransaction.Index = uint64(idx)

                block.UnfilteredTransactions.Add(deosTransaction);
            }

	        block.UnfilteredTransactionTraceCount = (uint) block.UnfilteredTransactionTraces.Count;
	        for idx, t := range ctx.block.UnfilteredTransactionTraces {
		        t.Index = uint64(idx)
		        t.BlockTime = ctx.block.Header.Timestamp
		        t.ProducerBlockId = ctx.block.Id
		        t.BlockNum = uint64(ctx.block.Number)

		        for _, actionTrace := range t.ActionTraces {
			        ctx.block.UnfilteredExecutedTotalActionCount++
			        if actionTrace.IsInput() {
				        ctx.block.UnfilteredExecutedInputActionCount++
			        }
		        }
	        }

//	        zlog.Debug("blocking until abi decoder has decoded every transaction pushed to it")

// TODO
            abiDecoder.endBlock(block);
	        /*if err != nil {
		        return null;, fmt.Errorf("abi decoding post-process failed: %w", err)
	        }*/

//	        zlog.Debug("abi decoder terminated all decoding operations, resetting block")
            ResetBlock();
            return block;
        }

        // Line format:
        //   APPLIED_TRANSACTION ${block_num} ${trace_hex}
        public Error ReadAppliedTransaction(string[] chunks)
        {
	        if (chunks.Length != 3)
            {
                return new Error($"expected 3 fields, got {chunks.Length}");
            }

            var blockNum = Convert.ToUInt32(chunks[1]);

            if (ActiveBlockNum != blockNum)
            {
                return new Error($"saw transactions from block {blockNum} while active block is {ActiveBlockNum}");
            }

            var trxTrace = Deserializer.Deserialize<TransactionTrace>(chunks[2].ToBytes());

	        return RecordTransaction(TransactionTraceToDEOS(trxTrace, ctx.conversionOptions...))
        }

        // Line formats:
        //  CREATION_OP ROOT ${action_id}
        //  CREATION_OP NOTIFY ${action_id}
        //  CREATION_OP INLINE ${action_id}
        //  CREATION_OP CFA_INLINE ${action_id}
        public Error ReadCreationOp(string[] chunks)
        {
	        if (chunks.Length != 3)
            {
                throw new Error($"expected 3 fields, got {chunks.Length}");
            }

            var kind = chunks[1];
	        if (kind != "ROOT" && kind != "NOTIFY" && kind != "INLINE" && kind != "CFA_INLINE")
            {
                throw new Error($"kind must be one of ROOT, NOTIFY, CFA_INLINE or INLINE, got: {kind}");
            }

            var actionIndex = Convert.ToInt32(chunks[2]);
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

	        return null;
        }

        // Line formats:
        //   DB_OP INS ${action_id} ${payer} ${table_code} ${scope} ${table_name} ${primkey} ${ndata}
        //   DB_OP UPD ${action_id} ${opayer}:${npayer} ${table_code} ${scope} ${table_name} ${primkey} ${odata}:${ndata}
        //   DB_OP REM ${action_id} ${payer} ${table_code} ${scope} ${table_name} ${primkey} ${odata}
        public Error ReadDbOp(string[] chunks)
        {
	        if (chunks.Length != 9)
            {
                throw new Error($"expected 9 fields, got {chunks.Length}");
            }

            var actionIndex = Convert.ToInt32(chunks[2]);
	        /*if err != nil {
		        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
	        }*/

            var opString = chunks[1];

            var op = DBOp.Types.Operation.Unknown;
            string oldData = null, newData = null;
            string oldPayer = null, newPayer = null;

            switch (opString)
            {
		        case "INS":
                    op = DBOp.Types.Operation.Insert;
                    newData = chunks[8];
                    newPayer = chunks[3];
					break;
    	        case "UPD":
                    op = DBOp.Types.Operation.Update;
                    var dataChunks = chunks[8].Split(':');
		            if (dataChunks.Length != 2)
                    {
                        throw new Error("should have old and new data in field 8, found only one");
                    }

                    oldData = dataChunks[0];
                    newData = dataChunks[1];

                    var payerChunks = chunks[3].Split(':');
		            if (payerChunks.Length != 2)
                    {
                        throw new Error("should have two payers in field 3, separated by a ':', found only one");
                    }

                    oldPayer = payerChunks[0];
                    newPayer = payerChunks[1];
					break;
    	        case "REM":
                    op = DBOp.Types.Operation.Remove;
                    oldData = chunks[8];
                    oldPayer = chunks[3];
					break;
    	        default:
                    throw new Error($"unknown operation: {opString}");
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

            RecordDBOp(new DBOp()
            {
                Operation = op,
                ActionIndex = (uint) actionIndex,
                OldPayer = oldPayer,
                NewPayer = newPayer,
                Code = chunks[4],
                Scope = chunks[5],
                TableName = chunks[6],
                PrimaryKey = chunks[7],
                OldData = ByteString.CopyFrom(oldBytes),
                NewData = ByteString.CopyFrom(newBytes),
            });

            return null;
        }

        // Line formats:
        //   DTRX_OP MODIFY_CANCEL ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
        //   DTRX_OP MODIFY_CREATE ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
        //   DTRX_OP CREATE        ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
        //   DTRX_OP CANCEL        ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
        //   DTRX_OP PUSH_CREATE   ${action_id} ${sender} ${sender_id} ${payer} ${published} ${delay} ${expiration} ${trx_id} ${trx}
        public Error ReadCreateOrCancelDTrxOp(string tag, string[] chunks)
        {
	        if (chunks.Length != 11)
            {
                throw new Error($"expected 11 fields, got {chunks.Length}");
            }

            var opString = chunks[1];
            var rawOp = Enum.Parse<DTrxOp.Types.Operation>(opString);//pbcodec.DTrxOp_Operation_value["OPERATION_" + opString]);
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

            var trxHex = chunks[10].ToBytes();
            /*if err != nil {
		        return fmt.Errorf("unable to decode signed transaction hex: %w", err)
	        }*/

            SignedTransaction signedTrx;// = new SignedTransaction();
	        if (op == DTrxOp.Types.Operation.PushCreate)
            {
                signedTrx = Deserializer.Deserialize<SignedTransaction>(trxHex);
                /*if err != nil {
			        return fmt.Errorf("unmarshal binary signed transaction: %w", err)
		        }*/
	        }
	        else
            {
                var trx = Deserializer.Deserialize<Transaction>(trxHex);
		        /*
		        if err != nil {
			        return fmt.Errorf("unmarshal binary transaction: %w", err)
		        }
		        */
                signedTrx = new SignedTransaction()
                {
                    Transaction = trx,
                };
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
                Transaction = SignedTransactionToDEOS(signedTrx),
            });

	        return null;
        }

        // Line format:
        //   DTRX_OP FAILED ${action_id}
        public Error ReadFailedDTrxOp(string[] chunks)
        {
	        if (chunks.Length != 3)
            {
                throw new Error($"expected 3 fields, got {chunks.Length}");
            }

            var actionIndex = Convert.ToInt32(chunks[2]);
	        /*if err != nil {
		        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
	        }*/

            RecordDTrxOp(new DTrxOp()
            {
                Operation = DTrxOp.Types.Operation.Failed,
                ActionIndex = (uint) actionIndex,
            });

	        return null;
        }

        // Line formats:
        //   FEATURE_OP ACTIVATE ${feature_digest} ${feature}
        public Error ReadFeatureOpActivate(string[] chunks)
        {
	        if (chunks.Length != 4)
            {
                throw new Error($"expected 4 fields, got {chunks.Length}");
            }

            var feature = JsonSerializer.Deserialize<Feature>(chunks[3]);
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

	        return null;
        }

        // Line formats:
        //   FEATURE_OP PRE_ACTIVATE ${action_id} ${feature_digest} ${feature}
        public Error ReadFeatureOpPreActivate(string[] chunks)
        {
	        if (chunks.Length != 5)
            {
                throw new Error($"expected 5 fields, got {chunks.Length}");
            }

            var actionIndex = Convert.ToInt32(chunks[2]);
	        /*if err != nil {
		        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
	        }*/

            var feature = JsonSerializer.Deserialize<Feature>(chunks[4]);
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

	        return null;
        }

        // Line formats: (the `[...]` represents optional fields)
        //   PERM_OP INS ${action_id} [${permission_id}] ${data}
        //   PERM_OP UPD ${action_id} [${permission_id}] ${data}
        //   PERM_OP REM ${action_id} [${permission_id}] ${data} <-- {"old": <old>, "new": <new>}
        public Error ReadPermOp(string[] chunks)
        {
            // TODO
	        /*chunks, err:= splitNToM(line, 4, 5)
	        if err != nil {
		        return err
	        }*/

            var actionIndex = Convert.ToInt32(chunks[2]);
	        /*if err != nil {
		        return fmt.Errorf("action_index is not a valid number, got: %q", chunks[2])
	        }*/

            var opString = chunks[1];
	        var dataChunk = chunks[3];
            ulong permissionID = 0;

	        // A `PERM_OP` with 5 fields have ["permission_id"] field in index #3 set and data chunk is actually index #4
	        if (chunks.Length == 5)
            {
                permissionID = Convert.ToUInt64(chunks[3]);
		        /*if err != nil {
			        return fmt.Errorf("permission_id is not a valid number, got: %q", chunks[3])
		        }*/
                dataChunk = chunks[4];
            }

            var op = PermOp.Types.Operation.Unknown;
            byte[] oldData = new byte[] { }, newData = new byte[] { };

	        switch (opString) {
		        case "INS":
			        op = PermOp.Types.Operation.Insert;
//		            newData = []byte(dataChunk)
                    newData = Encoding.ASCII.GetBytes(dataChunk);
                    break;
                case "UPD":
                    op = PermOp.Types.Operation.Update;
                    //   var oldJSONResult = gjson.Get(dataChunk, "old")
                    JsonDocument jsonDocument = JsonDocument.Parse(dataChunk);
                    if (!jsonDocument.RootElement.TryGetProperty("old", out var oldJsonResult))
                    {
                        throw new Error(
                            $"a PERM_OP UPD should JSON data should have an 'old' field, found none in: {dataChunk}");
                    }

		            if (!jsonDocument.RootElement.TryGetProperty("new", out var newJsonResult))
                    {
                        throw new Error(
                            $"a PERM_OP UPD should JSON data should have an 'new' field, found none in: {dataChunk}");
                    }

                    oldData = Encoding.ASCII.GetBytes(oldJsonResult.GetRawText());
    		        newData = Encoding.ASCII.GetBytes(newJsonResult.GetRawText());
                    break;
	            case "REM":
                    op = PermOp.Types.Operation.Remove;
                    oldData = Encoding.ASCII.GetBytes(dataChunk);
                    break;
    	        default:
                    throw new Error($"unknown PERM_OP op: {opString}");
	        }

            var permOp = new PermOp()
            {
                Operation = op,
                ActionIndex = (uint) actionIndex,
            };

	        if (newData.Length > 0)
            {
                var newPerm = Deserializer.Deserialize<PermissionObject>(newData);
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
                var oldPerm = new PermissionObject();
                JsonSerializer.Deserialize<PermissionObject>(oldData);
		        /*err = json.Unmarshal(oldData, &oldPerm)
		        if err != nil {
			        return fmt.Errorf("unmashal old perm data: %s", err)
		        }*/
                permOp.OldPerm = oldPerm;
                permOp.OldPerm.Id = permissionID;
            }

            RecordPermOp(permOp);

	        return null;
        }

        // Line format:
        //   RAM_OP ${action_index} ${unique_key} ${namespace} ${action} ${legacy_tag} ${payer} ${new_usage} ${delta}
        public Error ReadRamOp(string[] chunks)
        {
	        if (chunks.Length != 9)
            {
                throw new Error($"expected 9 fields, got {chunks.Length}");
            }

            var actionIndex = Convert.ToInt32(chunks[1]);

            var namespaceString = chunks[3];
	        var @namespace = Enum.Parse<RAMOp.Types.Namespace>(namespaceString);

            var actionString = chunks[4];
	        var action = Enum.Parse<RAMOp.Types.Action>(actionString);
	        /*if !ok {
	        return fmt.Errorf("action %q unknown", actionString)
	        }*/

            var operationString = chunks[5];
            var operation = Enum.Parse<RAMOp.Types.Operation>(operationString);
	        /*if !ok {
	        return fmt.Errorf("operation %q unknown", operationString)
	        }*/

            var usage = Convert.ToUInt64(chunks[7]);
	        /*if err != nil {
	        return fmt.Errorf("usage is not a valid number, got: %q", chunks[4])
	        }*/

            var delta = Convert.ToUInt64(chunks[8]);
	        /*if err != nil {
	        return fmt.Errorf("delta is not a valid number, got: %q", chunks[5])
	        }*/

            RecordRAMOp(new RAMOp()
            {
                ActionIndex = (uint) actionIndex,
                UniqueKey = chunks[2],
                Namespace = @namespace,
                Action = action,
                Operation = operation,
                Payer = chunks[6],
                Usage = usage,
                Delta = (long) delta,
            });
	        return null;
        }

        // Line format:
        //  Version 12
        //    DEEP_MIND_VERSION ${major_version}
        //
        //  Version 13
        //    DEEP_MIND_VERSION ${major_version} ${minor_version}
        public Error ReadDeepmindVersion(string[] chunks)
        {
            var majorVersion = chunks[1];
	        if (!inSupportedVersion(majorVersion)() {
                throw new Error(
                    $"deep mind reported version {majorVersion}, but this reader supports only {string.Join(supportedVersions, ", ")}");
            }

//	        zlog.Info("read deep mind version", zap.String("major_version", majorVersion))

	        return null;
        }

        public bool inSupportedVersion(string majorVersion)
        {
            foreach (var supportedVersion in supportedVersions)
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
        public Error ReadAbiStart(string[] chunks)
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
                    throw new Error($"expected to have either {{2}} or {{4}} fields, got {chunks.Length}");
	            }

//	        zlog.Info("read ABI start marker", logFields...)
//            abiDecoder.resetCache();
	        return null;
        }

        // Line format:
        //  Version 12
        //    ABIDUMP ABI ${block_num} ${contract} ${base64_abi}
        //
        //  Version 13
        //    ABIDUMP ABI ${contract} ${base64_abi}
        public Error ReadAbiDump(string[] chunks)
        {
            string contract, rawABI;
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

	        if (traceEnabled) {
//		        zlog.Debug("read initial ABI for contract", zap.String("contract", contract))
	        }

	        return abiDecoder.addInitialABI(contract, rawABI)
        }

        // Line format:
        //   RAM_CORRECTION_OP ${action_id} ${correction_id} ${unique_key} ${payer} ${delta}
        public Error ReadRamCorrectionOp(string[] chunks)
        {
	        if (chunks.Length != 6)
            {
                throw new Error($"expected 6 fields, got {chunks.Length}");
            }

	        // We assume ${action_id} will always be 0, since called from onblock, so that's why we do not process it

	        var delta = Convert.ToInt64(chunks[5]);
	        /*if err != nil {
		        return fmt.Errorf("delta not a valid number, got: %q", chunks[5])
	        }*/

            RecordRAMCorrectionOp(new RAMCorrectionOp()
            {
                CorrectionId = chunks[2],
                UniqueKey = chunks[3],
                Payer = chunks[4],
                Delta = delta,
            });
	        return null;
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
        public Error readRlimitOp(string[] chunks)
        {
            if (chunks.Length != 4)
            {
                throw new Error($"expected 4 fields, got {chunks.Length}");
            }

            var kindString = chunks[1];
	        var operationString = chunks[2];

            var operation = RlimitOp.Types.Operation.Unknown;
	        switch (operationString) {
		        case "INS":
                    operation = RlimitOp.Types.Operation.Insert;
                    break;
	            case "UPD":
                    operation = RlimitOp.Types.Operation.Update;
                    break;
	            default:
                    throw new Error($"operation {operationString} is unknown");
	        }

            var op = new RlimitOp() {Operation = operation};
            var data = chunks[3];

	        switch (kindString) {
		        case "CONFIG":
                    var rlimitConfig = JsonSerializer.Deserialize<RlimitConfig>(data);
                    op.Config = rlimitConfig;
                    break;
                case "STATE":
                    var rlimitState = JsonSerializer.Deserialize<RlimitState>(data);
                    op.State = rlimitState;
                    break;
	            case "ACCOUNT_LIMITS":
                    var rlimitAccountLimits = JsonSerializer.Deserialize<RlimitAccountLimits>(data);
                    op.AccountLimits = rlimitAccountLimits;
                    break;
                case "ACCOUNT_USAGE":
                    var rlimitAccountUsage = JsonSerializer.Deserialize<RlimitAccountUsage>(data);
                    op.AccountUsage = rlimitAccountUsage;
                    break;
                default:
                    throw new Error($"unknown kind: {kindString}");
	        }

            RecordRlimitOp(op);
            return null;
        }

        // Line formats:
        //   TBL_OP INS ${action_id} ${code} ${scope} ${table} ${payer}
        //   TBL_OP REM ${action_id} ${code} ${scope} ${table} ${payer}
        public Error readTableOp(string[] chunks)
        {
	        if (chunks.Length != 7)
            {
                throw new Error($"expected 7 fields, got {chunks.Length}");
            }

	        var actionIndex = Convert.ToInt32(chunks[2]);

            var opString = chunks[1];
            var op = TableOp.Types.Operation.Unknown;
	        switch (opString) {
		        case "INS":
                    op = TableOp.Types.Operation.Insert;
                    break;
	            case "REM":
                    op = TableOp.Types.Operation.Remove;
                    break;
	            default:
                    throw new Error($"unknown kind: {opString}");
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

	        return null;
        }

        // Line formats:
        //   TRX_OP CREATE onblock|onerror ${id} ${trx}
        public Error readTrxOp(string[] chunks)
        {
	        if (chunks.Length != 5)
            {
                throw new Error($"expected 5 fields, got {chunks.Length}");
            }

            var opString = chunks[1];
            var op = TrxOp.Types.Operation.Unknown;
	        switch (opString) {
		        case "CREATE":
                    op = TrxOp.Types.Operation.Create;
                    break;
    	        default:
                    throw new Error($"unknown kind: {opString}");
	        }

            var name = chunks[2];
            var trxID = chunks[3];

            var trxHex = chunks[4].ToBytes();

            var trx = Deserializer.Deserialize<SignedTransaction>(trxHex);

            RecordTrxOp(new TrxOp()
            {
                Operation = op,
                Name = name, // "onblock" or "onerror"
                TransactionId = trxID, // the hash of the transaction
                Transaction = SignedTransactionToDEOS(trx),
            });

	        return null;
        }
    }