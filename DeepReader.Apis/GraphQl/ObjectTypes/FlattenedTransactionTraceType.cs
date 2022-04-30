using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class FlattenedTransactionTraceType : ObjectType<FlattenedTransactionTrace>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedTransactionTrace> descriptor)
        {
            descriptor.Name("TransactionTrace");
            descriptor.Field(f => f.Id).Type<TransactionIdType>().Name("id");
            descriptor.Field(f => f.BlockNum).Type<UnsignedIntType>().Name("blockNum");
            descriptor.Field(f => f.Elapsed).Type<LongType>().Name("elapsed");
            descriptor.Field(f => f.NetUsage).Type<UnsignedLongType>().Name("netUsage");
            descriptor.Field(f => f.ActionTraces).Type<ListType<FlattenedActionTraceType>>().Name("actionTraces");
            descriptor.Field(f => f.DbOps).Type<ListType<DbOpType>>().Name("dbOps");
            descriptor.Field(f => f.TableOps).Type<ListType<TableOpType>>().Name("tableOps");
        }
    }
}