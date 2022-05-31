using DeepReader.Apis.GraphQl.CustomScalarTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class ActionType : ObjectType<Types.Eosio.Chain.Action>
    {
        protected override void Configure(IObjectTypeDescriptor<Types.Eosio.Chain.Action> descriptor)
        {
            descriptor.Name("Action");
            descriptor.Field(f => f.Account).Type<CustomScalarTypes.NameType>().Name("account");
            descriptor.Field(f => f.Name).Type<CustomScalarTypes.NameType>().Name("name");
            descriptor.Field(f => f.Authorization).Type<ListType<PermissionLevelType>>().Name("authorization");
            descriptor.Field(f => f.Authorization).Type<ListType<PermissionLevelType>>().Name("authorization");
            descriptor.Field(f => f.Data).Type<ActionDataBytesType>().Name("data");
            //descriptor.Field(f => f.Data.Json).Type<StringType>().Name("json"); // TODO
            //descriptor.Field(f => f.Data.Hex).Type<StringType>().Name("hexData"); // TODO
        }
    }
}