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
    // interesting, maybe for prometheus ?!
    // https://github.com/microsoft/FASTER/blob/29c4f58cc71962a4a9c573a4164f19851b275fe3/cs/samples/MemOnlyCache/CacheSizeTracker.cs
    public class BlockEvictionObserver : IObserver<IFasterScanIterator<BlockId, FlattenedBlock>>
    {
        private readonly FasterKV<BlockId, FlattenedBlock> _store;

        public BlockEvictionObserver(FasterKV<BlockId, FlattenedBlock> store)
        {
            _store = store;

            // Register subscriber to receive notifications of log evictions from memory
            _store.Log.SubscribeEvictions(this);

            // Include the separate read cache, if enabled
            if (_store.ReadCache != null)
                _store.ReadCache.SubscribeEvictions(this);
        }

        /// <summary>
        /// Subscriber to pages as they are getting evicted from main memory
        /// </summary>
        /// <param name="iter"></param>
        public void OnNext(IFasterScanIterator<BlockId, FlattenedBlock> iter)
        {
            while (iter.GetNext(out var recordinfo, out var blockId, out var block))
            {
                Log.Information($"Block {blockId} evicted from memory");
            }
        }

        /// <summary>
        /// OnCompleted
        /// </summary>
        public void OnCompleted() { }

        /// <summary>
        /// OnError
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            Log.Error(error,"");
        }
    }
}
