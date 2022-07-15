using DeepReader.Storage.Faster.Abis;
using DeepReader.Storage.TiDB.ValueConverters;
using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    public class DataContext : DbContext
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
            configurationBuilder.Properties<Types.EosTypes.Checksum256>().HaveConversion<Checksum256Converter>();
            configurationBuilder.Properties<Types.EosTypes.ActionDataBytes>().HaveConversion<ActionDataBytesConverter>();
            configurationBuilder.Properties<Types.EosTypes.PublicKey>().HaveConversion<PublicKeyConverter>();
            configurationBuilder.Properties<Types.EosTypes.Timestamp>().HaveConversion<TimestampConverter>();
            configurationBuilder.Properties<Types.Eosio.Chain.TransactionId>().HaveConversion<TransactionIdConverter>();
            configurationBuilder.Properties<Types.Fc.Crypto.Signature>().HaveConversion<SignatureConverter>();

            //configurationBuilder.Properties<Faster.Abis.AssemblyWrapper>().HaveConversion<AssemblyWrapperConverter>();

            configurationBuilder.Properties<ReadOnlyMemory<byte>>().HaveConversion<ReadOnlyMemoryByteConverter>();
            configurationBuilder.Properties<ReadOnlyMemory<char>>().HaveConversion<ReadOnlyMemoryCharConverter>();
            configurationBuilder.Properties<char[]>().HaveConversion<CharArrayConverter>();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Block>().Ignore(b => b.TransactionIds);
            modelBuilder.Entity<ActionTrace>().Ignore(a => a.CreatedActionIds);
            modelBuilder.Entity<TransactionTrace>().Ignore(t => t.ActionTraceIds);

            modelBuilder.Entity<ActionTrace>().HasKey(a => a.GlobalSequence);
            modelBuilder.Entity<Block>().HasKey(b => b.Number);
            modelBuilder.Entity<TransactionTrace>().HasKey(t => t.TransactionNum);
            
            modelBuilder.Entity<AbiCacheItem>().Property(a => a.AbiVersions).HasConversion<AbiVersionsConverter>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // ERROR
            // 'OnConfiguring' cannot be used to modify DbContextOptions when DbContext pooling is enabled.
            //optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}