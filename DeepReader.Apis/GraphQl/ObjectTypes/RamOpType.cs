using DeepReader.Types;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class RamOpType : ObjectType<RamOp>
    {
        protected override void Configure(IObjectTypeDescriptor<RamOp> descriptor)
        {
            descriptor.Name("RamOp");
            descriptor.Field(f => f.Operation).Type<StringType>().Name("operation");
            descriptor.Field(f => f.Payer).Type<CustomScalarTypes.NameType>().Name("payer");
            descriptor.Field(f => f.Delta).Type<LongType>().Name("delta");
            descriptor.Field(f => f.Usage).Type<UnsignedLongType>().Name("usage");
        }
    }
}