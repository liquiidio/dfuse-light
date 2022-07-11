using DeepReader.Storage.Faster.Abis;
using DeepReader.Types.EosTypes;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DeepReader.Storage.TiDB
{
    internal class AbiRepository
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public AbiRepository(IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task WriteAbi(AbiCacheItem abi, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            if (context is not null)
            {
                await context.Abis.AddAsync(abi);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                // Update ABI
            }
        }

        public async Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                var abi = await context.Abis.FirstOrDefaultAsync(a => a.Id == account.IntVal);

                if (abi is not null)
                    return (true, abi);
            }
            return (false, null!);
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            //using (AbiReaderSessionReadDurationSummary.NewTimer())
            //{
            //    if (!_sessionPool.TryGet(out var session))
            //        session = await _sessionPool.GetAsync().ConfigureAwait(false);
            //    var (status, output) = (await session.ReadAsync(account.IntVal)).Complete();
            //    _sessionPool.Return(session);

            //    if (status.Found && output.Value.AbiVersions.Any(av => av.Key <= globalSequence))
            //    {
            //        // returns the index of the Abi matching the globalSequence or binary complement of the next item (negative)
            //        var abiVersionIndex = output.Value.AbiVersions.Keys.ToList().BinarySearch(globalSequence);

            //        // if negative, revert the binary complement
            //        if (abiVersionIndex < 0)
            //            abiVersionIndex = ~abiVersionIndex;
            //        // we always want the previous Abi-version
            //        if (abiVersionIndex > 0)
            //            abiVersionIndex--;

            //        var abiVersionsArry = output.Value.AbiVersions.ToArray();
            //        if (abiVersionIndex >= 0 && abiVersionsArry.Length > abiVersionIndex)
            //            return (status.Found, abiVersionsArry[abiVersionIndex]);
            //    }
            //    return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            //}
            return (false, default!);
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            //using (AbiReaderSessionReadDurationSummary.NewTimer())
            //{
            //    if (!_sessionPool.TryGet(out var session))
            //        session = await _sessionPool.GetAsync().ConfigureAwait(false);
            //    var (status, output) = (await session.ReadAsync(account.IntVal)).Complete();
            //    _sessionPool.Return(session);

            //    if (status.Found && output.Value.AbiVersions.Count > 0)
            //    {
            //        return (status.Found, output.Value.AbiVersions.Last());
            //    }
            //    return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            //}
            return (false, default!);
        }
    }
}