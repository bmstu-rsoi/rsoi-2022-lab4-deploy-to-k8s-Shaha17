using LibrarySystem.RatingSystem.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LibrarySystem.RatingSystem.HealthChecks;

public class RatingSystemHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy());
    }
}