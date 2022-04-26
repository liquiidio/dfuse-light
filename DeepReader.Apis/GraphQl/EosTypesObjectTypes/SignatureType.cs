using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class SignatureType : ObjectType<Signature>
    {
        protected override void Configure(IObjectTypeDescriptor<Signature> descriptor)
        {
            descriptor.Field(f => f.StringVal).Type<StringType>();
        }
    }
}