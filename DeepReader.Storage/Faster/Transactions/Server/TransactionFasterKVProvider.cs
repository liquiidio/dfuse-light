using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    internal class TransactionFasterKVProvider : FasterKVProviderBase<TransactionId, TransactionTrace, TransactionId,
    TransactionTrace, TransactionServerFunctions, TransactionServerSerializer>
    {
        public TransactionFasterKVProvider(FasterKV<TransactionId, TransactionTrace> store, TransactionServerSerializer serializer,
      SubscribeKVBroker<TransactionId, TransactionTrace, TransactionId, IKeyInputSerializer<TransactionId, TransactionId>> kvBroker = null,
      SubscribeBroker<TransactionId, TransactionTrace, IKeySerializer<TransactionId>> broker = null, bool recoverStore = false,
      MaxSizeSettings maxSizeSettings = null)
      : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {
        }

        public override TransactionServerFunctions GetFunctions() => new();
    }
}