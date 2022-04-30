using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class FlattenedRamOpType : ObjectType<FlattenedRamOp>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedRamOp> descriptor)
        {
            descriptor.Name("RamOp");
            descriptor.Field(f => f.Operation).Type<StringType>().Name("operation");
            descriptor.Field(f => f.Payer).Type<CustomScalarTypes.NameType>().Name("payer");
            descriptor.Field(f => f.Delta).Type<LongType>().Name("delta");
            descriptor.Field(f => f.Usage).Type<UnsignedLongType>().Name("usage");
            descriptor.Field(f => f.Namespace).Type<StringType>().Name("namespace");
            descriptor.Field(f => f.Action).Type<StringType>().Name("action");
        }
    }
}