using DeepReader.Storage.Faster.Abis;
using DeepReader.Storage.TiDB.ValueConverters;
using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    internal class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Block> Blocks { get; set; }
        public DbSet<TransactionTrace> TransactionTraces { get; set; }
        public DbSet<ActionTrace> ActionTraces { get; set; }
        public DbSet<AbiCacheItem> Abis { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Types.EosTypes.Name>().HaveConversion<NameConverter>();
            configurationBuilder.Properties<ReadOnlyMemory<byte>>().HaveConversion<ReadOnlyMemoryByteConverter>();
            configurationBuilder.Properties<ReadOnlyMemory<char>>().HaveConversion<ReadOnlyMemoryCharConverter>();
            configurationBuilder.Properties<Types.EosTypes.ActionDataBytes>().HaveConversion<ActionDataBytesConverter>();
            configurationBuilder.Properties<ulong[]>().HaveConversion<ULongArrayConverter>();
            configurationBuilder.Properties<char[]>().HaveConversion<CharArrayConverter>();
        }
    }
}