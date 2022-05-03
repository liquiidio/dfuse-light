using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class PermissionLevelType : ObjectType<PermissionLevel>
    {
        protected override void Configure(IObjectTypeDescriptor<PermissionLevel> descriptor)
        {
            descriptor.Name("PermissionLevel");
            descriptor.Field(f => f.Actor).Type<CustomScalarTypes.NameType>().Name("actor");
            descriptor.Field(f => f.Permission).Type<CustomScalarTypes.NameType>().Name("permission");
        }
    }
}