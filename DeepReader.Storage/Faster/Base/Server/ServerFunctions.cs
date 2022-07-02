using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FASTER.core;

namespace DeepReader.Storage.Faster.Test.Server
{
    public class ServerFunctions<TKey, TValue> : IFunctions<TKey, TValue, TKey, TValue, long>
    {
        public void CheckpointCompletionCallback(int sessionID, string sessionName, CommitPoint commitPoint)
        {

        }

        public bool ConcurrentDeleter(ref TKey key, ref TValue value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool ConcurrentReader(ref TKey key, ref TKey input, ref TValue value,
            ref TValue dst, ref ReadInfo readInfo)
        {
            dst = value;
            return true;
        }

        public void ReadCompletionCallback(ref TKey key, ref TKey input, ref TValue output, long ctx, Status status,
            RecordMetadata recordMetadata)
        {

        }

        public bool ConcurrentWriter(ref TKey key, ref TKey input, ref TValue src,
            ref TValue dst, ref TValue output, ref UpsertInfo upsertInfo)
        {
            return true;
        }

        public bool CopyUpdater(ref TKey key, ref TKey input, ref TValue oldValue,
            ref TValue newValue, ref TValue output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void DisposeCopyUpdater(ref TKey key, ref TKey input, ref TValue oldValue,
            ref TValue newValue, ref TValue output, ref RMWInfo rmwInfo)
        {

        }

        public void DisposeDeserializedFromDisk(ref TKey key, ref TValue value)
        {

        }

        public void DisposeInitialUpdater(ref TKey key, ref TKey input, ref TValue value,
            ref TValue output, ref RMWInfo rmwInfo)
        {

        }

        public void DisposeSingleDeleter(ref TKey key, ref TValue value, ref DeleteInfo deleteInfo)
        {

        }

        public void DisposeSingleWriter(ref TKey key, ref TKey input, ref TValue src,
            ref TValue dst, ref TValue output, ref UpsertInfo upsertInfo, WriteReason reason)
        {

        }

        public bool InitialUpdater(ref TKey key, ref TKey input, ref TValue value,
            ref TValue output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool InPlaceUpdater(ref TKey key, ref TKey input, ref TValue value,
            ref TValue output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void RMWCompletionCallback(ref TKey key, ref TKey input, ref TValue output, long ctx, Status status,
            RecordMetadata recordMetadata)
        {
        }


        public bool NeedCopyUpdate(ref TKey key, ref TKey input, ref TValue oldValue,
            ref TValue output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool NeedInitialUpdate(ref TKey key, ref TKey input, ref TValue output,
            ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void PostCopyUpdater(ref TKey key, ref TKey input, ref TValue oldValue,
            ref TValue newValue, ref TValue output, ref RMWInfo rmwInfo)
        {

        }

        public void PostInitialUpdater(ref TKey key, ref TKey input, ref TValue value,
            ref TValue output, ref RMWInfo rmwInfo)
        {

        }

        public void PostSingleDeleter(ref TKey key, ref DeleteInfo deleteInfo)
        {

        }

        public void PostSingleWriter(ref TKey key, ref TKey input, ref TValue src,
            ref TValue dst, ref TValue output, ref UpsertInfo upsertInfo, WriteReason reason)
        {

        }

        public bool SingleDeleter(ref TKey key, ref TValue value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool SingleReader(ref TKey key, ref TKey input, ref TValue value,
            ref TValue dst, ref ReadInfo readInfo)
        {
            dst = value;
            return true;
        }

        public bool SingleWriter(ref TKey key, ref TKey input, ref TValue src,
            ref TValue dst, ref TValue output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            dst = src;
            return true;
        }
    }
}
