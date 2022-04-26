namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class ActionType : ObjectType<Types.Eosio.Chain.Action>
    {
        protected override void Configure(IObjectTypeDescriptor<Types.Eosio.Chain.Action> descriptor)
        {
            descriptor.Field(f => f.Account).Type<NameType>().Name("Account");
            descriptor.Field(f => f.Name).Type<NameType>().Name("Name");
            descriptor.Field(f => f.Authorization).Type<ListType<PermissionLevelType>>().Name("Authorization");
            descriptor.Field(f => f.Data).Type<ActionDataBytesType>().Name("Data");
        }
    }
}