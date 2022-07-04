using DeepReader.Storage.Faster.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace DeepReader.Storage.HealthChecks.Faster
{
    public class MaxTransactionsCacheEntriesHealthCheck : IHealthCheck
    {
        public IOptionsMonitor<FasterStandaloneOptions> StorageOptionsMonitor { get; }

        public MaxTransactionsCacheEntriesHealthCheck(IOptionsMonitor<FasterStandaloneOptions> storageOptionsMonitor)
        {
            StorageOptionsMonitor = storageOptionsMonitor;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("MaxTransactionsCacheEntries: " + StorageOptionsMonitor.CurrentValue.MaxTransactionsCacheEntries));
        }
    }
}