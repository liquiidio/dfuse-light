namespace DeepReader.Storage.Faster.Options
{
    public class FasterStandaloneOptions : IFasterStorageOptions
    {
        public string BlockStoreDir { get; set; }
        public string TransactionStoreDir { get; set; }
        public string ActionTraceStoreDir { get; set; }
        public string AbiStoreDir { get; set; }
        public FasterMode Mode { get; set; }
        public long MaxBlocksCacheEntries { get; set; }
        public long MaxTransactionsCacheEntries { get; set; }
        public long MaxAbiCacheEntries { get; set; }
        public long MaxActionTracesCacheEntries { get; set; }
        public bool UseReadCache { get; set; }
        public int? LogCheckpointInterval { get; set; }
        public bool FlushAfterCheckpoint { get; set; }
        public int IndexCheckpointMultiplier { get; set; }
    }
}
