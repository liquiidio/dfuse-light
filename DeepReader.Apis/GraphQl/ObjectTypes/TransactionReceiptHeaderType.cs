using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class TransactionReceiptHeaderType : ObjectType<TransactionReceiptHeader>
    {
        protected override void Configure(IObjectTypeDescriptor<TransactionReceiptHeader> descriptor)
        {
            descriptor.Name("TransactionReceiptHeader");
            descriptor.Field(f => f.Status).Type<TransactionIdType>().Name("status");
            descriptor.Field(f => f.CpuUsageUs).Type<BlockType>().Name("cpuUsageMicroSeconds"); // TODO, this is always null atm
            descriptor.Field(f => f.NetUsageWords).Type<UnsignedIntType>().Name("netUsageWords");
        }
    }
}