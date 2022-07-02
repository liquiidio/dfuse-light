using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.Transactions
{
    public abstract class TransactionStoreBase
    {
        internal int _sessionCount;

        internal readonly FasterStorageOptions _options;

        internal ITopicEventSender _eventSender;
        internal MetricsCollector _metricsCollector;

        internal static readonly SummaryConfiguration SummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary WritingTransactionDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_write_transaction_duration", "Summary of time to store transactions to Faster", SummaryConfiguration);
        internal static readonly Summary StoreLogMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_log_memory_size_bytes", "Summary of the faster transaction store log memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_read_cache_memory_size_bytes", "Summary of the faster transaction store read cache memory size in bytes", SummaryConfiguration);
        internal static readonly Summary StoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_transaction_store_read_cache_memory_size_bytes", "Summary of the faster transaction store entry count", SummaryConfiguration);
        internal static readonly Summary StoreTakeLogCheckpointDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_transaction_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster transaction store", SummaryConfiguration);
        internal static readonly Summary StoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster transaction store", SummaryConfiguration);
        internal static readonly Summary StoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster transaction store", SummaryConfiguration);
        internal static readonly Summary TransactionReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_transaction_get_by_id_duration", "Summary of time to try get transaction trace by id", SummaryConfiguration);

        protected TransactionStoreBase(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            _options = options;
            _eventSender = eventSender;
            _metricsCollector = metricsCollector;
            _metricsCollector.CollectMetricsHandler += CollectObservableMetrics;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);
        public abstract Task<Status> WriteTransaction(TransactionTrace transaction);
        public abstract Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId);
    }
}