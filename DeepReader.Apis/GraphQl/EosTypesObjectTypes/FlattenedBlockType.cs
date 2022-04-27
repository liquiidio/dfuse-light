using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class FlattenedBlockType : ObjectType<FlattenedBlock>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedBlock> descriptor)
        {
            descriptor.Field(f => f.Id).Type<Checksum256Type>().Name("Id");
            descriptor.Field(f => f.Number).Type<UnsignedIntType>().Name("Number");
            descriptor.Field(f => f.Producer).Type<NameType>().Name("Producer");
            descriptor.Field(f => f.ProducerSignature).Type<SignatureType>().Name("ProducerSignature");
            descriptor.Field(f => f.TransactionIds).Type<ListType<TransactionIdType>>().Name("TransationIds");
            descriptor.Field(f => f.Transactions).Type<ListType<FlattenedTransactionTraceType>>().Name("Transactions");
        }
    }
}