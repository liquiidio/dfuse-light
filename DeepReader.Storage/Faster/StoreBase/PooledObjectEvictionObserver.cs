using DeepReader.Types.Other;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.StoreBase
{
    internal class PooledObjectEvictionObserver<TKey, TValue> : IObserver<IFasterScanIterator<TKey, TValue>>
        where TValue : PooledObject<TValue>, IParentPooledObject<TValue>, new()
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            Log.Error(error, "");
        }

        public void OnNext(IFasterScanIterator<TKey, TValue> iter)
        {
            while (iter.GetNext(out RecordInfo info, out TKey key, out TValue value))
            {
                if (info.IsLocked)
                    continue;

                value.ReturnToPoolRecursive();
            }
        }
    }
}