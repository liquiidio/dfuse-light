using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DeepReader.Storage.HealthChecks.Faster
{
    public class TransactionsIndexedHealthCheck : IHealthCheck
    {
        public IStorageAdapter Storage { get; }

        public TransactionsIndexedHealthCheck(IStorageAdapter storage)
        {
            Storage = storage;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("TransactionsIndexed: " + Storage.TransactionsIndexed));
        }
    }
}