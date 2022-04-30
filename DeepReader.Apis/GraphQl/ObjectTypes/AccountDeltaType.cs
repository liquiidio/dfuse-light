using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class AccountDeltaType : ObjectType<AccountDelta>
    {
        protected override void Configure(IObjectTypeDescriptor<AccountDelta> descriptor)
        {
            descriptor.Name("AccountDelta");
            descriptor.Field(f => f.Account).Type<CustomScalarTypes.NameType>().Name("account");
            descriptor.Field(f => f.Delta).Type<LongType>().Name("delta");
        }
    }
}