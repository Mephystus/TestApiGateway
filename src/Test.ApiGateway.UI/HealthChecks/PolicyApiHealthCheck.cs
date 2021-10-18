// -------------------------------------------------------------------------------------
//  <copyright file="PolicyApiHealthCheck.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

namespace Test.ApiGateway.UI.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Policy.Api.Client.Interfaces;

/// <summary>
/// Defines the health check implementation for the dependency: 'Policy API'
/// </summary>
public class PolicyApiHealthCheck : IHealthCheck
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<PolicyApiHealthCheck> _logger;
    
    /// <summary>
    /// The policy API client.
    /// </summary>
    private readonly IPolicyApiClient _policyApiClient;

    /// <summary>
    /// Initialises a new instance of the <see cref="PolicyApiHealthCheck" /> class.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger{PolicyApiHealthCheck}"/></param>
    /// <param name="policyApiClient">An instance of <see cref="IPolicyApiClient"/></param>
    public PolicyApiHealthCheck(
        ILogger<PolicyApiHealthCheck> logger,
        IPolicyApiClient policyApiClient)
    {
        _logger = logger;
        _policyApiClient = policyApiClient;
    }

    /// <summary>
    /// Runs the health check, returning the status of the component being checked.
    /// </summary>
    /// <param name="context">A context object associated with the current execution.</param>
    /// <param name="cancellationToken">The token that can be used to cancel the health check.</param>
    /// <returns>A <see cref="Task"/> that completes when the health check has finished,
    /// yielding the status of the component being checked.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _policyApiClient.PingAsync();

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            string message = "Failed policy API health check!";

            _logger.LogError(ex, message);

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                description: message,
                exception: ex,
                data: null);
        }
    }
}