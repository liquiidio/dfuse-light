using DeepReader.Storage.Faster.Blocks.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Blocks.Server
{
    internal class BlockServerFunctions : IFunctions<long, Block, BlockInput, BlockOutput, long>
    {
        public void CheckpointCompletionCallback(int sessionID, string sessionName, CommitPoint commitPoint)
        {
            
        }

        public bool ConcurrentDeleter(ref long key, ref Block value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool ConcurrentReader(ref long key, ref BlockInput input, ref Block value, ref BlockOutput dst, ref ReadInfo readInfo)
        {
            dst.Value = value;
            return true;
        }

        public bool ConcurrentWriter(ref long key, ref BlockInput input, ref Block src, ref Block dst, ref BlockOutput output, ref UpsertInfo upsertInfo)
        {
            return true;
        }

        public bool CopyUpdater(ref long key, ref BlockInput input, ref Block oldValue, ref Block newValue, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void DisposeCopyUpdater(ref long key, ref BlockInput input, ref Block oldValue, ref Block newValue, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            
        }

        public void DisposeDeserializedFromDisk(ref long key, ref Block value)
        {
            
        }

        public void DisposeInitialUpdater(ref long key, ref BlockInput input, ref Block value, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            
        }

        public void DisposeSingleDeleter(ref long key, ref Block value, ref DeleteInfo deleteInfo)
        {
            
        }

        public void DisposeSingleWriter(ref long key, ref BlockInput input, ref Block src, ref Block dst, ref BlockOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            
        }

        public bool InitialUpdater(ref long key, ref BlockInput input, ref Block value, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool InPlaceUpdater(ref long key, ref BlockInput input, ref Block value, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool NeedCopyUpdate(ref long key, ref BlockInput input, ref Block oldValue, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public bool NeedInitialUpdate(ref long key, ref BlockInput input, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            return true;
        }

        public void PostCopyUpdater(ref long key, ref BlockInput input, ref Block oldValue, ref Block newValue, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            
        }

        public void PostInitialUpdater(ref long key, ref BlockInput input, ref Block value, ref BlockOutput output, ref RMWInfo rmwInfo)
        {
            
        }

        public void PostSingleDeleter(ref long key, ref DeleteInfo deleteInfo)
        {
            
        }

        public void PostSingleWriter(ref long key, ref BlockInput input, ref Block src, ref Block dst, ref BlockOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            
        }

        public void ReadCompletionCallback(ref long key, ref BlockInput input, ref BlockOutput output, long ctx, Status status, RecordMetadata recordMetadata)
        {
            
        }

        public void RMWCompletionCallback(ref long key, ref BlockInput input, ref BlockOutput output, long ctx, Status status, RecordMetadata recordMetadata)
        {
            
        }

        public bool SingleDeleter(ref long key, ref Block value, ref DeleteInfo deleteInfo)
        {
            return true;
        }

        public bool SingleReader(ref long key, ref BlockInput input, ref Block value, ref BlockOutput dst, ref ReadInfo readInfo)
        {
            dst.Value = value;
            return true;
        }

        public bool SingleWriter(ref long key, ref BlockInput input, ref Block src, ref Block dst, ref BlockOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            return true;
        }
    }
}