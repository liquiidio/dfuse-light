using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.FlattenedTypes;
using Nest;

namespace DeepReader.Storage.Elastic
{
    internal class ElasticStorage : IStorageAdapter
    {
        private ElasticClient elasticClient;

        public ElasticStorage()
        {
            //var settings = new ConnectionSettings(new Uri("http://example.com:9200")).DefaultIndex("people");
            var connectionSettings = new ConnectionSettings();
            elasticClient = new ElasticClient(connectionSettings);

        }

        public async Task StoreBlockAsync(FlattenedBlock block) // compress, store, index
        {
            await elasticClient.IndexDocumentAsync(new FlattenedBlockWrapper(){ Block = block });
        }

        public async Task StoreTransactionAsync(FlattenedTransactionTrace transactionTrace)  // compress, store, index
        {
            await elasticClient.IndexDocumentAsync(new FlattenedTransactionTraceWrapper(){ TransactionTrace = transactionTrace });
        }
    }
}
