using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.ActionTraces
{
    public abstract class ActionTraceStoreBase
    {
        internal readonly FasterStorageOptions _options;

        internal int _sessionCount;

        internal readonly ITopicEventSender _eventSender;

        internal MetricsCollector _metricsCollector;

        internal static readonly SummaryConfiguration SummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary WritingActionTraceDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_write_action_trace_duration", "Summary of time to store actionTraces to Faster", SummaryConfiguration);
        internal static readonly Summary StoreLogMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_log_memory_size_bytes", "Summary of the faster actionTrace store log memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Summary of the faster actionTrace store read cache memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Summary of the faster actionTrace store entry count", SummaryConfiguration);
        internal static readonly Summary StoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster actionTrace store", SummaryConfiguration);
        internal static readonly Summary StoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster actionTrace store", SummaryConfiguration);
        internal static readonly Summary StoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster actionTrace store", SummaryConfiguration);
        internal static readonly Summary ActionTraceReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_action_trace_get_by_id_duration", "Summary of time to try get actionTrace trace by id", SummaryConfiguration);

        protected ActionTraceStoreBase(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            _options = options;
            _eventSender = eventSender;
            _metricsCollector = metricsCollector;
            _metricsCollector.CollectMetricsHandler += CollectObservableMetrics;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);

        public abstract Task<Status> WriteActionTrace(ActionTrace actionTrace);

        public abstract Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence);
    }
}
