using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Storage
{
    public interface IStorageAdapter
    {
        Task StoreBlockAsync(FlattenedBlock block);

        Task StoreTransactionAsync(FlattenedTransactionTrace transactionTrace);
    }
}
