using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class ActionReceiptType : ObjectType<ActionReceipt>
    {
        protected override void Configure(IObjectTypeDescriptor<ActionReceipt> descriptor)
        {
            descriptor.Name("ActionReceipt");
            descriptor.Field(f => f.Receiver).Type<CustomScalarTypes.NameType>().Name("receiver");
            descriptor.Field(f => f.ActionDigest).Type<CustomScalarTypes.Checksum256Type>().Name("actionDigest");
            descriptor.Field(f => f.GlobalSequence).Type<UnsignedLongType>().Name("globalSequence");
            descriptor.Field(f => f.ReceiveSequence).Type<UnsignedLongType>().Name("receiveSequence");
            descriptor.Field(f => f.AuthSequence).Type<ListType<TransactionTraceAuthSequenceType>>().Name("authSequence");
            descriptor.Field(f => f.CodeSequence).Type<UnsignedIntType>().Name("codeSequence");
            descriptor.Field(f => f.AbiSequence).Type<UnsignedIntType>().Name("abiSequence");
        }
    }
}