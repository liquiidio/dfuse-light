using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class FlattenedTransactionTraceType : ObjectType<FlattenedTransactionTrace>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedTransactionTrace> descriptor)
        {
            // Todo (Haron) How is uint and ulong represented in GraphQL
            descriptor.Field(f => f.Id).Type<TransactionIdType>().Name("Id");
            descriptor.Field(f => f.BlockNum).Type<AnyType>().Name("BlockNum");
            descriptor.Field(f => f.Elapsed).Type<LongType>().Name("Elapsed");
            descriptor.Field(f => f.NetUsage).Type<AnyType>().Name("NetUsage");
            descriptor.Field(f => f.ActionTraces).Type<ListType<FlattenedActionTraceType>>().Name("ActionTraces");
            descriptor.Field(f => f.DbOps).Type<ListType<DbOpType>>().Name("DbOps");
            descriptor.Field(f => f.TableOps).Type<ListType<TableOpType>>().Name("TableOps");
        }
    }
}