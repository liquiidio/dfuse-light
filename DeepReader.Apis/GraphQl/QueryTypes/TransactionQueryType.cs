using DeepReader.Apis.GraphQl.ObjectTypes;
using DeepReader.Apis.GraphQl.Queries;

namespace DeepReader.Apis.GraphQl.QueryTypes
{
    internal class TransactionQueryType : ObjectTypeExtension<TransactionQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<TransactionQuery> descriptor)
        {
            descriptor.Name("Query");
            descriptor
                .Field(f => f.GetTransaction(default!, default!)!)
                .Argument("transaction_id", a => a.Type<StringType>())
                .Type<FlattenedTransactionTraceType>()
                .Name("getTransaction");
        }
    }
}