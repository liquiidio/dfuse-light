using DeepReader.Storage.Faster.Stores.Abis.Standalone;
using FASTER.core;

namespace DeepReader.Storage.Faster.Stores.Abis.Server
{
    internal sealed class AbiServerFunctions : IFunctions<ulong, AbiCacheItem, AbiInput, AbiOutput, long>
    {
        public void CheckpointCompletionCallback(int sessionID, string sessionName, CommitPoint commitPoint)
        {

        }

        public bool ConcurrentDeleter(ref ulong key, ref AbiCacheItem value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool ConcurrentReader(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput dst, ref ReadInfo readInfo)
        {
            dst.Value = value;
            return true;
        }

        public bool ConcurrentWriter(ref ulong key, ref AbiInput input, ref AbiCacheItem src, ref AbiCacheItem dst, ref AbiOutput output, ref UpsertInfo upsertInfo)
        {
            return true;
        }

        public bool CopyUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem oldValue, ref AbiCacheItem newValue, ref AbiOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void DisposeCopyUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem oldValue, ref AbiCacheItem newValue, ref AbiOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void DisposeDeserializedFromDisk(ref ulong key, ref AbiCacheItem value)
        {

        }

        public void DisposeInitialUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void DisposeSingleDeleter(ref ulong key, ref AbiCacheItem value, ref DeleteInfo deleteInfo)
        {

        }

        public void DisposeSingleWriter(ref ulong key, ref AbiInput input, ref AbiCacheItem src, ref AbiCacheItem dst, ref AbiOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {

        }

        public bool InitialUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool InPlaceUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool NeedCopyUpdate(ref ulong key, ref AbiInput input, ref AbiCacheItem oldValue, ref AbiOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool NeedInitialUpdate(ref ulong key, ref AbiInput input, ref AbiOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void PostCopyUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem oldValue, ref AbiCacheItem newValue, ref AbiOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void PostInitialUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
        {

        }

        public void PostSingleDeleter(ref ulong key, ref DeleteInfo deleteInfo)
        {

        }

        public void PostSingleWriter(ref ulong key, ref AbiInput input, ref AbiCacheItem src, ref AbiCacheItem dst, ref AbiOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {

        }

        public void ReadCompletionCallback(ref ulong key, ref AbiInput input, ref AbiOutput output, long ctx, Status status, RecordMetadata recordMetadata)
        {

        }

        public void RMWCompletionCallback(ref ulong key, ref AbiInput input, ref AbiOutput output, long ctx, Status status, RecordMetadata recordMetadata)
        {

        }

        public bool SingleDeleter(ref ulong key, ref AbiCacheItem value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool SingleReader(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput dst, ref ReadInfo readInfo)
        {
            dst.Value = value;
            return true;
        }

        public bool SingleWriter(ref ulong key, ref AbiInput input, ref AbiCacheItem src, ref AbiCacheItem dst, ref AbiOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            return true;
        }
    }
}