using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.Subscriptions
{
    internal class TransactionSubscription
    {
        [Subscribe]
        public FlattenedTransactionTrace? TransactionAdded([EventMessage] FlattenedTransactionTrace transaction) => transaction;
    }
}