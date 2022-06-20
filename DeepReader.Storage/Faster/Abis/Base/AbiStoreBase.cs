using DeepReader.Storage.Faster.Abis.Standalone;
using DeepReader.Storage.Options;
using DeepReader.Types.EosTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using System.Reflection;

namespace DeepReader.Storage.Faster.Abis.Base
{
    public abstract class AbiStoreBase
    {
        internal int _sessionCount;

        internal readonly FasterStorageOptions _options;

        internal readonly ITopicEventSender _eventSender;
        internal MetricsCollector _metricsCollector;

        internal static readonly SummaryConfiguration SummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary WritingAbiDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_write_abi_duration", "Summary of time to store abis to Faster", SummaryConfiguration);
        internal static readonly Summary StoreLogMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_log_memory_size_bytes", "Summary of the faster abi store log memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_read_cache_memory_size_bytes", "Summary of the faster abi store read cache memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_abi_store_read_cache_memory_size_bytes", "Summary of the faster abi store entry count", SummaryConfiguration);
        internal static readonly Summary StoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster abi store", SummaryConfiguration);
        internal static readonly Summary StoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster abi store", SummaryConfiguration);
        internal static readonly Summary StoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_abi_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster abi store", SummaryConfiguration);
        internal static readonly Summary AbiReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_abi_get_by_id_duration", "Summary of time to try get abi trace by id", SummaryConfiguration);

        protected AbiStoreBase(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            _options = options;
            _eventSender = eventSender;
            _metricsCollector = metricsCollector;
            _metricsCollector.CollectMetricsHandler += CollectObservableMetrics;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);
        public abstract Task<Status> WriteAbi(AbiCacheItem abi);
        public abstract Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly);
        public abstract Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account);
        public abstract Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence);
        public abstract Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account);
    }
}