using DeepReader.Types.EosTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class ActionDataBytesType : ObjectType<ActionDataBytes>
    {
        // Todo (Haron)
        protected override void Configure(IObjectTypeDescriptor<ActionDataBytes> descriptor)
        {
            descriptor.Field(f => f.Binary).Type<ByteArrayType>().Name("Binary");
        }
    }
}