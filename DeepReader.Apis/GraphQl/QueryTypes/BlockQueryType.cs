﻿using DeepReader.Apis.GraphQl.ObjectTypes;
using DeepReader.Apis.GraphQl.Queries;

namespace DeepReader.Apis.GraphQl.QueryTypes
{
    internal class BlockQueryType : ObjectTypeExtension<BlockQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<BlockQuery> descriptor)
        {
            descriptor.Name("Query");
            descriptor
                .Field(f => f.GetBlock(default, default!)!)
                .Argument("block_num", a => a.Type<UnsignedIntType>())
                .Type<BlockType>()
                .Name("getBlock");
            descriptor
                .Field(f => f.GetBlockWithTraces(default, default!))
                .Argument("block_num", a => a.Type<UnsignedIntType>())
                .Type<BlockType>()
                .Name("getBlockWithTraces");
        }
    }
}