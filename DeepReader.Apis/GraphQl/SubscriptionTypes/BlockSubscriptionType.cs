using DeepReader.Apis.GraphQl.ObjectTypes;
using DeepReader.Apis.GraphQl.Subscriptions;

namespace DeepReader.Apis.GraphQl.SubscriptionTypes
{
    internal class BlockSubscriptionType : ObjectTypeExtension<BlockSubscription>
    {
        protected override void Configure(IObjectTypeDescriptor<BlockSubscription> descriptor)
        {
            descriptor.Name("Subscription");
            descriptor
                .Field(f => f.BlockAdded(default!))
                .Type<FlattenedBlockType>()
                .Name("subscribeToBlockAdded");
        }
    }
}