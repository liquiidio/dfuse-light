using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.ActionTraces
{
    public abstract class ActionTraceStoreBase
    {
        internal readonly FasterStorageOptions Options;

        internal int SessionCount;

        internal readonly ITopicEventSender EventSender;

        internal MetricsCollector MetricsCollector;

        internal static readonly SummaryConfiguration TypeSummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary TypeWritingActionTraceDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_write_action_trace_duration", "Summary of time to store actionTraces to Faster", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreLogMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_log_memory_size_bytes", "Summary of the faster actionTrace store log memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Summary of the faster actionTrace store read cache memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Summary of the faster actionTrace store entry count", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster actionTrace store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster actionTrace store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster actionTrace store", TypeSummaryConfiguration);
        internal static readonly Summary TypeActionTraceReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_action_trace_get_by_id_duration", "Summary of time to try get actionTrace trace by id", TypeSummaryConfiguration);

        protected ActionTraceStoreBase(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            Options = options;
            EventSender = eventSender;
            MetricsCollector = metricsCollector;
            MetricsCollector.CollectMetricsHandler += CollectObservableMetrics;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);

        public abstract Task<Status> WriteActionTrace(ActionTrace actionTrace);

        public abstract Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence);
    }
}
