using DeepReader.Storage.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace DeepReader.Storage.HealthChecks.Faster
{
    public class MaxBlocksCacheEntriesHealthCheck : IHealthCheck
    {
        public IOptionsMonitor<FasterStorageOptions> StorageOptionsMonitor { get; }

        public MaxBlocksCacheEntriesHealthCheck(IOptionsMonitor<FasterStorageOptions> storageOptionsMonitor)
        {
            StorageOptionsMonitor = storageOptionsMonitor;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("MaxBlocksCacheEntries: " + StorageOptionsMonitor.CurrentValue.MaxBlocksCacheEntries));
        }
    }
}