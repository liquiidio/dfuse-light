using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.ActionTraces
{
    internal class ActionTraceEvictionObserver : IObserver<IFasterScanIterator<ulong, ActionTrace>>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            Log.Error(error,"");
        }

        public void OnNext(IFasterScanIterator<ulong, ActionTrace> iter)
        {
            while (iter.GetNext(out RecordInfo info, out ulong key, out ActionTrace value))
            {
                if (info.IsLocked)
                    continue;

                value.ReturnToPoolRecursive();
                
                Console.WriteLine($"Block {key} evicted ");
            }
        }
    }
}
