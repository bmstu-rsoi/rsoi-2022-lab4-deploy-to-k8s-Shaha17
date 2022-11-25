using LibrarySystem.ReservationSystem.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LibrarySystem.ReservationSystem.HealthChecks;

public class ReservationSystemHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy());

    }
}