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
            descriptor.Field(f => f.ActionDigest).Type<CustomScalarTypes.NameType>().Name("digest");
            descriptor.Field(f => f.GlobalSequence).Type<ListType<PermissionLevelType>>().Name("globalSequence");
            descriptor.Field(f => f.CodeSequence).Type<ActionDataBytesType>().Name("codeSequence");
            descriptor.Field(f => f.AbiSequence).Type<ActionDataBytesType>().Name("abiSequence");
        }
    }
}