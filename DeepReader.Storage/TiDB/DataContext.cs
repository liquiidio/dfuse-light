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
    }
}