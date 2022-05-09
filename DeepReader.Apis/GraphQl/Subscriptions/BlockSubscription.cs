using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.Subscriptions
{
    internal class BlockSubscription
    {
        [Subscribe]
        public FlattenedBlock? BlockAdded([EventMessage] FlattenedBlock block) => block;
    }
}