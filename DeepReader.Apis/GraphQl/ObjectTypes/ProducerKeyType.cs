using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class ProducerKeyType : ObjectType<ProducerKey>
    {
        protected override void Configure(IObjectTypeDescriptor<ProducerKey> descriptor)
        {
            descriptor.Name("ProducerKey");
            descriptor.Field(f => f.AccountName).Type<CustomScalarTypes.NameType>().Name("name");
            descriptor.Field(f => f.BlockSigningKey).Type<CustomScalarTypes.NameType>().Name("key");
        }
    }
}