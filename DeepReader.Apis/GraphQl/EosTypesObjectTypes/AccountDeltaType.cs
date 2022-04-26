using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class AccountDeltaType : ObjectType<AccountDelta>
    {
        protected override void Configure(IObjectTypeDescriptor<AccountDelta> descriptor)
        {
            descriptor.Field(f => f.Account).Type<NameType>().Name("Account");
            descriptor.Field(f => f.Delta).Type<LongType>().Name("Delta");
        }
    }
}