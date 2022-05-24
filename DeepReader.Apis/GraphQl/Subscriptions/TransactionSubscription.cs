using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.Subscriptions
{
    internal class TransactionSubscription
    {
        [Subscribe]
        public TransactionTrace? TransactionAdded([EventMessage] TransactionTrace transaction) => transaction;
    }
}