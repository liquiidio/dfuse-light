using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.client;

namespace DeepReader.Storage.Faster.Transactions.Client
{
    internal class TransactionClientFunctions : ICallbackFunctions<TransactionId, TransactionTrace, TransactionInput, TransactionOutput, TransactionContext>
    {
        public void DeleteCompletionCallback(ref TransactionId key, TransactionContext ctx)
        {

        }

        public void PublishCompletionCallback(ref TransactionId key, ref TransactionTrace value, TransactionContext ctx)
        {

        }

        public void ReadCompletionCallback(ref TransactionId key, ref TransactionInput input, ref TransactionOutput output, TransactionContext ctx, Status status)
        {

        }

        public void RMWCompletionCallback(ref TransactionId key, ref TransactionInput input, ref TransactionOutput output, TransactionContext ctx, Status status)
        {

        }

        public void SubscribeCallback(ref TransactionId key, ref TransactionTrace value, TransactionContext ctx)
        {

        }

        public void SubscribeKVCallback(ref TransactionId key, ref TransactionInput input, ref TransactionOutput output, TransactionContext ctx, Status status)
        {

        }

        public void UpsertCompletionCallback(ref TransactionId key, ref TransactionTrace value, TransactionContext ctx)
        {

        }
    }
}