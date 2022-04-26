using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class PermissionLevelType : ObjectType<PermissionLevel>
    {
        protected override void Configure(IObjectTypeDescriptor<PermissionLevel> descriptor)
        {
            descriptor.Field(f => f.Actor).Type<NameType>().Name("Actor");
            descriptor.Field(f => f.Permission).Type<NameType>().Name("Permission");
        }
    }
}