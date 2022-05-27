using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Blocks
{
    internal class BlockEvictionObserver : IObserver<IFasterScanIterator<BlockId, Block>>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            Log.Error(error,"");
        }

        public void OnNext(IFasterScanIterator<BlockId, Block> iter)
        {
            while (iter.GetNext(out RecordInfo info, out BlockId key, out Block value))
            {
                // If it is not Invalid, we must Seal it so there is no possibility it will be missed while we're in the process
                // of transferring it to the Lock Table. Use manualLocking as we want to transfer the locks, not drain them.
                if (info.IsLocked)
                    continue;

                Console.WriteLine($"key.id evicted {key.Id}");
            }
        }
    }
}
