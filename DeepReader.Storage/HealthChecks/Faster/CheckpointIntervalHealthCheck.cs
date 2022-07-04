using DeepReader.Storage.Faster.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace DeepReader.Storage.HealthChecks.Faster
{
    public class CheckpointIntervalHealthCheck : IHealthCheck
    {
        public IOptionsMonitor<FasterStandaloneOptions> StorageOptionsMonitor { get; }

        public CheckpointIntervalHealthCheck(IOptionsMonitor<FasterStandaloneOptions> storageOptionsMonitor)
        {
            StorageOptionsMonitor = storageOptionsMonitor;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("CheckpointInterval: " + StorageOptionsMonitor.CurrentValue.LogCheckpointInterval));
        }
    }
}