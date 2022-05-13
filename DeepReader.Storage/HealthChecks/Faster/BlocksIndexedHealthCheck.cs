using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DeepReader.Storage.HealthChecks.Faster
{
    public class BlocksIndexedHealthCheck : IHealthCheck
    {
        public IStorageAdapter Storage { get; }

        public BlocksIndexedHealthCheck(IStorageAdapter storage)
        {
            Storage = storage;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("BlocksIndexed: " + Storage.BlocksIndexed));
        }
    }
}