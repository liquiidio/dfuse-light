using DeepReader.Apis.GraphQl.EosTypesObjectTypes;
using DeepReader.Apis.GraphQl.Queries;

namespace DeepReader.Apis.GraphQl.QueryTypes
{
    internal class BlockQueryType : ObjectTypeExtension
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Query");
            descriptor.Field("GetBlock").ResolveWith<BlockQuery>(q => q.GetBlock()).Type<FlattenedBlockType>().Name("GetBlockById");
        }
    }
}