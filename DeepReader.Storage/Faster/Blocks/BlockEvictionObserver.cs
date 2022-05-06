using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Blocks
{
    internal class BlockEvictionObserver : IObserver<IFasterScanIterator<BlockId, FlattenedBlock>>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            Log.Error(error,"");
        }

        public void OnNext(IFasterScanIterator<BlockId, FlattenedBlock> iter)
        {
            while (iter.GetNext(out RecordInfo info, out BlockId key, out FlattenedBlock value))
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
