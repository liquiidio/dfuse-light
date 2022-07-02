using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Blocks
{
    public abstract class BlockStoreBase
    {
        internal int _sessionCount;

        internal readonly FasterStorageOptions _options;

        internal ITopicEventSender _eventSender;
        internal MetricsCollector _metricsCollector;

        internal static readonly SummaryConfiguration SummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary WritingBlockDuration =
            Metrics.CreateSummary("deepreader_storage_faster_write_block_duration", "Summary of time to store blocks to Faster", SummaryConfiguration);
        internal static readonly Summary StoreLogMemorySizeBytesSummary =
           Metrics.CreateSummary("deepreader_storage_faster_block_store_log_memory_size_bytes", "Summary of the faster block store log memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Summary of the faster block store read cache memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Summary of the faster block store entry count", SummaryConfiguration);
        internal static readonly Summary StoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster block store", SummaryConfiguration);
        internal static readonly Summary StoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster block store", SummaryConfiguration);
        internal static readonly Summary StoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster block store", SummaryConfiguration);
        internal static readonly Summary BlockReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_block_get_by_id_duration", "Summary of time to try get block by id", SummaryConfiguration);

        protected BlockStoreBase(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            _options = options;
            _eventSender = eventSender;
            _metricsCollector = metricsCollector;
            _metricsCollector.CollectMetricsHandler += CollectObservableMetrics;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);
        public abstract Task<Status> WriteBlock(Block block);
        public abstract Task<(bool, Block)> TryGetBlockById(uint blockNum);

    }
}