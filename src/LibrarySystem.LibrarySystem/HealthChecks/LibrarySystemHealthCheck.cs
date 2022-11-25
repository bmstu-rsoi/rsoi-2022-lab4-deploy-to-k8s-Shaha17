using System.Runtime.Serialization.Formatters;
using LibrarySystem.LibrarySystem.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LibrarySystem.LibrarySystem.HealthChecks;

public class LibrarySystemHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy());
    }
}