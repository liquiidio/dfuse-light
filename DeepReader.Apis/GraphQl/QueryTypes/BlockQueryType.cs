using DeepReader.Apis.GraphQl.EosTypesObjectTypes;
using DeepReader.Apis.GraphQl.Queries;

namespace DeepReader.Apis.GraphQl.QueryTypes
{
    internal class BlockQueryType : ObjectTypeExtension
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Query");
            descriptor
                .Field("GetBlock")
                .Argument("block_num", a => a.Type<UnsignedIntType>())
                .ResolveWith<BlockQuery>(q => q.GetBlock(default, default!))
                .Type<FlattenedBlockType>()
                .Name("GetBlockById");
        }
    }
}