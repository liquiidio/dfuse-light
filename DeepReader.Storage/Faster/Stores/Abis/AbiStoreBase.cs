using System.Reflection;
using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.Stores.Abis.Custom;
using DeepReader.Types.EosTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.Abis
{
    public abstract class AbiStoreBase
    {
        internal int SessionCount;

        internal readonly IFasterStorageOptions Options;

        internal readonly ITopicEventSender EventSender;
        internal MetricsCollector MetricsCollector;

        internal static readonly SummaryConfiguration TypeSummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary TypeWritingAbiDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_write_abi_duration", "Summary of time to store abis to Faster", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreLogMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_log_memory_size_bytes", "Summary of the faster abi store log memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_read_cache_memory_size_bytes", "Summary of the faster abi store read cache memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_abi_store_read_cache_memory_size_bytes", "Summary of the faster abi store entry count", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster abi store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster abi store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster abi store", TypeSummaryConfiguration);
        internal static readonly Summary TypeAbiReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_abi_get_by_id_duration", "Summary of time to try get abi trace by id", TypeSummaryConfiguration);

        protected AbiStoreBase(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            Options = options;
            EventSender = eventSender;
            MetricsCollector = metricsCollector;
            MetricsCollector.CollectMetricsHandler += CollectObservableMetrics;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);
        public abstract Task<Status> WriteAbi(AbiCacheItem abi);
        public abstract Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly);
        public abstract Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account);
        public abstract Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence);
        public abstract Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account);
    }
}