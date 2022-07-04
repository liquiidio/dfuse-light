using DeepReader.Storage.Faster.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.Blocks
{
    public abstract class BlockStoreBase
    {
        internal readonly IFasterStorageOptions Options;

        internal int SessionCount;

        internal readonly ITopicEventSender EventSender;
        internal MetricsCollector MetricsCollector;

        internal static readonly SummaryConfiguration TypeSummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary TypeWritingBlockDuration =
            Metrics.CreateSummary("deepreader_storage_faster_write_block_duration", "Summary of time to store blocks to Faster", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreLogMemorySizeBytesSummary =
           Metrics.CreateSummary("deepreader_storage_faster_block_store_log_memory_size_bytes", "Summary of the faster block store log memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Summary of the faster block store read cache memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Summary of the faster block store entry count", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster block store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster block store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster block store", TypeSummaryConfiguration);
        internal static readonly Summary TypeBlockReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_block_get_by_id_duration", "Summary of time to try get block by id", TypeSummaryConfiguration);

        public long BlocksIndexed;

        protected BlockStoreBase(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            Options = options;
            EventSender = eventSender;
            MetricsCollector = metricsCollector;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);
        public abstract Task<Status> WriteBlock(Block block);
        public abstract Task<(bool, Block)> TryGetBlockById(uint blockNum);

    }
}