using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.Subscriptions
{
    internal class BlockSubscription
    {
        [Subscribe]
        public Block? BlockAdded([EventMessage] Block block) => block;
    }
}