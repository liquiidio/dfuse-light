﻿using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    internal class TransactionServerFunctions : IFunctions<TransactionId, TransactionTrace, TransactionInput, TransactionOutput, long>
    {
        public void CheckpointCompletionCallback(int sessionID, string sessionName, CommitPoint commitPoint)
        {

        }

        public bool ConcurrentDeleter(ref TransactionId key, ref TransactionTrace value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool ConcurrentReader(ref TransactionId key, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput dst, ref ReadInfo readInfo)
        {
            dst.Value = value;
            return true;
        }

        public bool ConcurrentWriter(ref TransactionId key, ref TransactionInput input, ref TransactionTrace src, ref TransactionTrace dst, ref TransactionOutput output, ref UpsertInfo upsertInfo)
        {
            return true;
        }

        public bool CopyUpdater(ref TransactionId key, ref TransactionInput input, ref TransactionTrace oldValue, ref TransactionTrace newValue, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void DisposeCopyUpdater(ref TransactionId key, ref TransactionInput input, ref TransactionTrace oldValue, ref TransactionTrace newValue, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void DisposeDeserializedFromDisk(ref TransactionId key, ref TransactionTrace value)
        {
            throw new NotImplementedException();
        }

        public void DisposeInitialUpdater(ref TransactionId key, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void DisposeSingleDeleter(ref TransactionId key, ref TransactionTrace value, ref DeleteInfo deleteInfo)
        {

        }

        public void DisposeSingleWriter(ref TransactionId key, ref TransactionInput input, ref TransactionTrace src, ref TransactionTrace dst, ref TransactionOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {

        }

        public bool InitialUpdater(ref TransactionId key, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool InPlaceUpdater(ref TransactionId key, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool NeedCopyUpdate(ref TransactionId key, ref TransactionInput input, ref TransactionTrace oldValue, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool NeedInitialUpdate(ref TransactionId key, ref TransactionInput input, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void PostCopyUpdater(ref TransactionId key, ref TransactionInput input, ref TransactionTrace oldValue, ref TransactionTrace newValue, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void PostInitialUpdater(ref TransactionId key, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void PostSingleDeleter(ref TransactionId key, ref DeleteInfo deleteInfo)
        {

        }

        public void PostSingleWriter(ref TransactionId key, ref TransactionInput input, ref TransactionTrace src, ref TransactionTrace dst, ref TransactionOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {

        }

        public void ReadCompletionCallback(ref TransactionId key, ref TransactionInput input, ref TransactionOutput output, long ctx, Status status, RecordMetadata recordMetadata)
        {

        }

        public void RMWCompletionCallback(ref TransactionId key, ref TransactionInput input, ref TransactionOutput output, long ctx, Status status, RecordMetadata recordMetadata)
        {

        }

        public bool SingleDeleter(ref TransactionId key, ref TransactionTrace value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool SingleReader(ref TransactionId key, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput dst, ref ReadInfo readInfo)
        {
            dst.Value = value;
            return true;
        }

        public bool SingleWriter(ref TransactionId key, ref TransactionInput input, ref TransactionTrace src, ref TransactionTrace dst, ref TransactionOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            return true;
        }
    }
}