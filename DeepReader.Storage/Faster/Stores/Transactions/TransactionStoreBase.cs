using DeepReader.Storage.Faster.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.Transactions
{
    public abstract class TransactionStoreBase
    {
        internal readonly IFasterStorageOptions Options;

        internal int SessionCount;

        internal readonly ITopicEventSender EventSender;
        internal MetricsCollector MetricsCollector;

        internal static readonly SummaryConfiguration TypeSummaryConfiguration = new SummaryConfiguration()
        { MaxAge = TimeSpan.FromSeconds(30) };

        internal static readonly Summary TypeWritingTransactionDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_write_transaction_duration", "Summary of time to store transactions to Faster", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreLogMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_log_memory_size_bytes", "Summary of the faster transaction store log memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_read_cache_memory_size_bytes", "Summary of the faster transaction store read cache memory size in bytes", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_transaction_store_read_cache_memory_size_bytes", "Summary of the faster transaction store entry count", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeLogCheckpointDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_transaction_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster transaction store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster transaction store", TypeSummaryConfiguration);
        internal static readonly Summary TypeStoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_transaction_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster transaction store", TypeSummaryConfiguration);
        internal static readonly Summary TypeTransactionReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_transaction_get_by_id_duration", "Summary of time to try get transaction trace by id", TypeSummaryConfiguration);

        internal long TransactionsIndexed;

        protected TransactionStoreBase(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            Options = options;
            EventSender = eventSender;
            MetricsCollector = metricsCollector;
        }

        protected abstract void CollectObservableMetrics(object? sender, EventArgs e);
        public abstract Task<Status> WriteTransaction(TransactionTrace transaction);
        public abstract Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId);
    }
}