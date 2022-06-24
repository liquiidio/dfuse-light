using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Blocks.Standalone
{
    internal class BlockEvictionObserver : IObserver<IFasterScanIterator<long, Block>>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            Log.Error(error, "");
        }

        public void OnNext(IFasterScanIterator<long, Block> iter)
        {
            while (iter.GetNext(out RecordInfo info, out long key, out Block value))
            {
                if (info.IsLocked)
                    continue;

                value.ReturnToPoolRecursive();
            }
        }
    }
}
