using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    internal class TransactionRepository
    {
        private readonly DataContext _context;

        public TransactionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task WriteTransaction(TransactionTrace transaction)
        {
            await _context.TransactionTraces.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId)
        {
            var transaction = await _context.TransactionTraces.FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction is null)
                return (false, null!);
            return (true, transaction);
        }
    }
}