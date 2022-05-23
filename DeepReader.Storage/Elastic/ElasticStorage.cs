using DeepReader.Storage.Faster.Abis;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using Microsoft.Extensions.Options;
using Nest;
using System.Reflection;

namespace DeepReader.Storage.Elastic
{
    internal class ElasticStorage : IStorageAdapter
    {
        private readonly ElasticClient _elasticClient;

        private ElasticStorageOptions _elasticStorageOptions;

        public ElasticStorage(IOptionsMonitor<ElasticStorageOptions> storageOptionsMonitor)
        {
            _elasticStorageOptions = storageOptionsMonitor.CurrentValue;
            storageOptionsMonitor.OnChange(OnElasticStorageOptionsChanged);

            //var settings = new ConnectionSettings(new Uri("http://example.com:9200")).DefaultIndex("people");
            var connectionSettings = new ConnectionSettings();
            _elasticClient = new ElasticClient(connectionSettings);
        }

        private void OnElasticStorageOptionsChanged(ElasticStorageOptions newOptions)
        {
            _elasticStorageOptions = newOptions;
        }

        public Task StoreBlockAsync(Block block) // compress, store, index
        {
            throw new NotImplementedException();
        }

        public Task StoreTransactionAsync(TransactionTrace transactionTrace)  // compress, store, index
        {
            throw new NotImplementedException();
        }

        public Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId)
        {
            throw new NotImplementedException();
        }

        public Task StoreActionTraceAsync(ActionTrace actionTrace)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId, bool includeActionTraces = false)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAbi(Types.EosTypes.Name account, ulong globalSequence, Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Types.EosTypes.Name account)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Types.EosTypes.Name account, ulong globalSequence)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Types.EosTypes.Name account)
        {
            throw new NotImplementedException();
        }
    }
}
