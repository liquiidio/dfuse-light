using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class FlattenedBlockType : ObjectType<FlattenedBlock>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedBlock> descriptor)
        {
            descriptor.Name("Block");
            descriptor.Field(f => f.Id).Type<Checksum256Type>().Name("id");
            descriptor.Field(f => f.Number).Type<UnsignedIntType>().Name("number");
            descriptor.Field(f => f.Producer).Type<CustomScalarTypes.NameType>().Name("producer");
            descriptor.Field(f => f.ProducerSignature).Type<SignatureType>().Name("producerSignature");
            descriptor.Field(f => f.TransactionIds).Type<ListType<TransactionIdType>>().Name("transationIds");
            descriptor.Field(f => f.Transactions).Type<ListType<FlattenedTransactionTraceType>>().Name("transactions");
        }
    }
}