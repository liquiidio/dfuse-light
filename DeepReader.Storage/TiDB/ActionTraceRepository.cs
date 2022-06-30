using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    internal class ActionTraceRepository
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public ActionTraceRepository(IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task WriteActionTrace(ActionTrace actionTrace, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                await context.ActionTraces.AddAsync(actionTrace);
                await context.SaveChangesAsync();
            }
        }

        public async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                var actionTrace = await context.ActionTraces.FirstOrDefaultAsync(a => a.GlobalSequence == globalSequence);

                if (actionTrace is not null)
                    return (true, actionTrace);
            }

            return (false, null!);
        }
    }
}