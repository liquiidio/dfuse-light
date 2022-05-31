using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class BlockType : ObjectType<Block>
    {
        protected override void Configure(IObjectTypeDescriptor<Block> descriptor)
        {
            descriptor.Name("Block");
            descriptor.Field(f => f.Id).Type<Checksum256Type>().Name("id");
            descriptor.Field(f => f.Number).Type<UnsignedIntType>().Name("number");
            descriptor.Field(f => f.Timestamp).Type<TimestampType>().Name("timestamp");
            descriptor.Field(f => f.Producer).Type<CustomScalarTypes.NameType>().Name("producer");
            descriptor.Field(f => f.Confirmed).Type<UnsignedShortType>().Name("confirmed");
            descriptor.Field(f => f.Previous).Type<Checksum256Type>().Name("previous");
            descriptor.Field(f => f.TransactionMroot).Type<Checksum256Type>().Name("transactionMroot");
            descriptor.Field(f => f.ActionMroot).Type<Checksum256Type>().Name("actionMroot");
            descriptor.Field(f => f.ScheduleVersion).Type<UnsignedIntType>().Name("scheduleVersion");
            descriptor.Field(f => f.NewProducers).Type<ProducerScheduleType>().Name("newProducer");
            descriptor.Field(f => f.ProducerSignature).Type<SignatureType>().Name("producerSignature");
            descriptor.Field(f => f.TransactionIds).Type<ListType<TransactionIdType>>().Name("transationIds");
            descriptor.Field(f => f.Transactions).Type<ListType<TransactionTraceType>>().Name("transactionTraces");
        }
    }
}