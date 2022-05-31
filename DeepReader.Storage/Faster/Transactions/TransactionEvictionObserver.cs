using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Transactions
{
    internal class TransactionEvictionObserver : IObserver<IFasterScanIterator<TransactionId, TransactionTrace>>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            Log.Error(error,"");
        }

        public void OnNext(IFasterScanIterator<TransactionId, TransactionTrace> iter)
        {
            while (iter.GetNext(out RecordInfo info, out TransactionId key, out TransactionTrace value))
            {
                if (info.IsLocked)
                    continue;

                value.ReturnToPoolRecursive();
            }
        }
    }
}
