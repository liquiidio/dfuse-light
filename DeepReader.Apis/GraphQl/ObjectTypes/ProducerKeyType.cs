using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class ProducerKeyType : ObjectType<ProducerKey>
    {
        protected override void Configure(IObjectTypeDescriptor<ProducerKey> descriptor)
        {
            descriptor.Name("ProducerKey");
            descriptor.Field(f => f.AccountName).Type<CustomScalarTypes.NameType>().Name("accountName");
            descriptor.Field(f => f.BlockSigningKey).Type<CustomScalarTypes.PublicKeyType>().Name("blockSigningKey");
        }
    }
}