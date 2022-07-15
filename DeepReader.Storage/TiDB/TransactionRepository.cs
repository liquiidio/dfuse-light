﻿using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    internal class TransactionRepository
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public TransactionRepository(IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task WriteTransaction(TransactionTrace transaction, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                await context.TransactionTraces.AddAsync(transaction);
                await context.SaveChangesAsync();
            }
        }

        public async Task WriteTransactions(IEnumerable<TransactionTrace> transactions, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                await context.TransactionTraces.AddRangeAsync(transactions);
                await context.SaveChangesAsync();
            }
        }

        public async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId, bool includeActionTraces, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                TransactionTrace? transaction = null;

                if (includeActionTraces)
                {
                    transaction = await context.TransactionTraces
                        .Include(t => t.ActionTraces)
                        .FirstOrDefaultAsync(t => t.Id == transactionId);
                }
                else
                {
                    transaction = await context.TransactionTraces
                        .FirstOrDefaultAsync(t => t.Id == transactionId);
                }

                if (transaction is not null)
                    return (true, transaction);
            }
            return (false, null!);
        }
    }
}