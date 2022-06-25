using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.client;

namespace DeepReader.Storage.Faster.Transactions.Client
{
    // notes Output is TransactionTrace or Wrapper, must be same type as Output in TransactionClientSerializer
    // FASTER.client.ClientSession<T> internally calls ClientSerializer.ReadOutput and then ReadCompletionCallback etc.

    internal class TransactionClientFunctions : ICallbackFunctions<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace, TransactionContext>
    {
        public void ReadCompletionCallback(ref TransactionId key, ref TransactionInput input, ref TransactionTrace output,
            TransactionContext ctx, Status status)
        {
            throw new NotImplementedException();
        }

        public void ReadCompletionCallback(ref TransactionId key, ref TransactionTrace input, ref TransactionTrace output,
            TransactionContext ctx, Status status)
        {
            throw new NotImplementedException();
        }

        public void UpsertCompletionCallback(ref TransactionId key, ref TransactionTrace value, TransactionContext ctx)
        {
            throw new NotImplementedException();
        }

        public void RMWCompletionCallback(ref TransactionId key, ref TransactionTrace input, ref TransactionTrace output,
            TransactionContext ctx, Status status)
        {
            throw new NotImplementedException();
        }

        public void DeleteCompletionCallback(ref TransactionId key, TransactionContext ctx)
        {
            throw new NotImplementedException();
        }

        public void SubscribeKVCallback(ref TransactionId key, ref TransactionTrace input, ref TransactionTrace output,
            TransactionContext ctx, Status status)
        {
            throw new NotImplementedException();
        }

        public void PublishCompletionCallback(ref TransactionId key, ref TransactionTrace value, TransactionContext ctx)
        {
            throw new NotImplementedException();
        }

        public void SubscribeCallback(ref TransactionId key, ref TransactionTrace value, TransactionContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}