using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Storage.Options
{
    public class FasterStorageOptions
    {
        public string BlockStoreDir { get; set; }
        public string TransactionStoreDir { get; set; }

        public FasterMode Mode { get; set; }

        public long MaxBlocksCacheEntries { get; set; }
        public long MaxTransactionsCacheEntries { get; set; }
        public bool UseReadCache { get; set; }
    }

    public enum FasterMode
    {
        MEM_ONLY,
        LRU_CACHE,
        LRU_CACHE_REQUESTED_ONLY
    }
}
