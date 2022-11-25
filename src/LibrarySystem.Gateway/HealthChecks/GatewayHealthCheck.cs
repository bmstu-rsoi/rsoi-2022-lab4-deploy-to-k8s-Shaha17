using LibrarySystem.Gateway.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LibrarySystem.Gateway.HealthChecks;

public class GatewayHealthCheck : IHealthCheck
{

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy());
    }
}