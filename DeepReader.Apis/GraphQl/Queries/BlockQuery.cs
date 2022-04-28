using DeepReader.Storage;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.Queries
{
    internal class BlockQuery
    {
        //public FlattenedBlock GetBlock()
        //{
        //    return new FlattenedBlock
        //    {
        //        Id = "",
        //        Number = 7000,
        //        Producer = "eosio",
        //        ProducerSignature = "SIG_K1_5F2txY4N1gUV9jUbCuevUAtwEmcgSrRfC4P4KrNwhNcJyZvafGuwFYjcwAUcvXR2djh5cvtVYrBUFWX6PKQ81xQ4SeWw2",
        //        TransactionIds = new Types.Eosio.Chain.TransactionId[]
        //        {
        //            "fa44b35d7c26bfec9df5fda9f3b0d41b2f821f4d6724b5d3b4ba6f3175efedf6"
        //        }
        //    };
        //}

        public async Task<FlattenedBlock?> GetBlock(uint block_num, [Service]IStorageAdapter storage)
        {
            var (found, block) = await storage.GetBlockAsync(block_num);
            if (found)
                return block;
            return null;
        }
    }
}