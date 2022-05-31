using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class TransactionTraceType : ObjectType<TransactionTrace>
    {
        protected override void Configure(IObjectTypeDescriptor<TransactionTrace> descriptor)
        {
            descriptor.Name("TransactionTrace");
            descriptor.Field(f => f.Id).Type<TransactionIdType>().Name("id");
            descriptor.Field(f => f.BlockNum).Type<UnsignedIntType>().Name("blockNum");
            //            descriptor.Field(f => f.Block).Type<BlockType>().Name("block"); // TODO, this is always null atm
            //            descriptor.Field(f => f.Status).Type<int>().Name("status"); // TODO

            descriptor.Field(f => f.Receipt).Type<TransactionReceiptHeaderType>().Name("receipt");
            descriptor.Field(f => f.Elapsed).Type<LongType>().Name("elapsed");
            descriptor.Field(f => f.NetUsage).Type<UnsignedLongType>().Name("netUsage");
            descriptor.Field(f => f.Scheduled).Type<BooleanType>().Name("scheduled");
            descriptor.Field(f => f.ActionTraces).Type<ListType<ActionTraceType>>().Name("actionTraces"); // instead of executedActions, matchingActions etc. we just have actionTraces
            descriptor.Field(f => f.ActionTraceIds).Type<ListType<UnsignedLongType>>().Name("actionTraceIds");
        }
    }
}