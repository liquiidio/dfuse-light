using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class TransactionTraceAuthSequenceType : ObjectType<TransactionTraceAuthSequence>
    {
        protected override void Configure(IObjectTypeDescriptor<TransactionTraceAuthSequence> descriptor)
        {
            descriptor.Name("TransactionTraceAuthSequence");
            descriptor.Field(f => f.Account).Type<CustomScalarTypes.NameType>().Name("account");
            descriptor.Field(f => f.Sequence).Type<UnsignedLongType>().Name("sequence");
        }
    }
}
