using DeepReader.Apis.GraphQl.ObjectTypes;
using DeepReader.Apis.GraphQl.Subscriptions;

namespace DeepReader.Apis.GraphQl.SubscriptionTypes
{
    internal class TransactionSubscriptionType : ObjectTypeExtension<TransactionSubscription>
    {
        protected override void Configure(IObjectTypeDescriptor<TransactionSubscription> descriptor)
        {
            descriptor.Name("Subscription");
            descriptor
                .Field(f => f.TransactionAdded(default!))
                .Type<TransactionTraceType>()
                .Name("subscribeToTransactionAdded");
        }
    }
}