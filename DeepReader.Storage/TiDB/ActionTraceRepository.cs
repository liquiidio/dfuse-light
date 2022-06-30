using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    internal class ActionTraceRepository
    {
        private readonly DataContext _context;

        public ActionTraceRepository(DataContext context)
        {
            _context = context;
        }

        public async Task WriteActionTrace(ActionTrace actionTrace)
        {
            await _context.ActionTraces.AddAsync(actionTrace);
            await _context.SaveChangesAsync();
        }

        public async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
        {
            var actionTrace = await _context.ActionTraces.FirstOrDefaultAsync(a => a.GlobalSequence == globalSequence);

            if (actionTrace is null)
                return (false, null!);

            return (true, actionTrace);
        }
    }
}