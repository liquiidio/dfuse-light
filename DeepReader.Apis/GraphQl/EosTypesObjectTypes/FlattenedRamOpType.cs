using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class FlattenedRamOpType : ObjectType<FlattenedRamOp>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedRamOp> descriptor)
        {
            descriptor.Field(f => f.Operation).Type<StringType>().Name("Operation");
            descriptor.Field(f => f.Payer).Type<NameType>().Name("Payer");
            descriptor.Field(f => f.Delta).Type<LongType>().Name("Delta");
            descriptor.Field(f => f.Usage).Type<UnsignedLongType>().Name("Usage");
            descriptor.Field(f => f.Namespace).Type<StringType>().Name("Namespace");
            descriptor.Field(f => f.Action).Type<StringType>().Name("Action");
        }
    }
}