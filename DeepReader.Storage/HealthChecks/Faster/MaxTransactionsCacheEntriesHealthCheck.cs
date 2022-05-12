using DeepReader.Storage.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace DeepReader.Storage.HealthChecks.Faster
{
    public class MaxTransactionsCacheEntriesHealthCheck : IHealthCheck
    {
        public IOptionsMonitor<FasterStorageOptions> StorageOptionsMonitor { get; }

        public MaxTransactionsCacheEntriesHealthCheck(IOptionsMonitor<FasterStorageOptions> storageOptionsMonitor)
        {
            StorageOptionsMonitor = storageOptionsMonitor;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("MaxTransactionsCacheEntries: " + StorageOptionsMonitor.CurrentValue.MaxTransactionsCacheEntries));
        }
    }
}