using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class TransactionIdType : ObjectType<TransactionId>
    {
        protected override void Configure(IObjectTypeDescriptor<TransactionId> descriptor)
        {
            descriptor.Field(f => f.StringVal).Type<StringType>();
        }
    }
}