using DeepReader.Types.EosTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class NameType : ObjectType<Name>
    {
        protected override void Configure(IObjectTypeDescriptor<Name> descriptor)
        {
            descriptor.Field(f => f.StringVal).Type<StringType>();
        }
    }
}