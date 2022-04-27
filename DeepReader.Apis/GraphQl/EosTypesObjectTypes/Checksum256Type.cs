using DeepReader.Types.EosTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class Checksum256Type : ObjectType<Checksum256>
    {
        protected override void Configure(IObjectTypeDescriptor<Checksum256> descriptor)
        {
            descriptor.Field(f => f.StringVal).Type<StringType>();
        }
    }
}