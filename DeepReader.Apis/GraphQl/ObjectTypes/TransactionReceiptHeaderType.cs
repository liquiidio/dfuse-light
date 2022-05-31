using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class TransactionReceiptHeaderType : ObjectType<TransactionReceiptHeader>
    {
        protected override void Configure(IObjectTypeDescriptor<TransactionReceiptHeader> descriptor)
        {
            descriptor.Name("TransactionReceiptHeader");
            descriptor.Field(f => f.Status).Type<StringType>().Name("status");
            descriptor.Field(f => f.CpuUsageUs).Type<UnsignedIntType>().Name("cpuUsageMicroSeconds"); // TODO, this is always null atm
            descriptor.Field(f => f.NetUsageWords).Type<VarInt32Type>().Name("netUsageWords");
        }
    }
}