// -------------------------------------------------------------------------------------
//  <copyright file="CustomerApiHealthCheck.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

namespace Test.ApiGateway.UI.HealthChecks;

using Customer.Api.Client.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Defines the health check implementation for the dependency: 'Customer API'
/// </summary>
public class CustomerApiHealthCheck : IHealthCheck
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<CustomerApiHealthCheck> _logger;

    /// <summary>
    /// The customer API client.
    /// </summary>
    private readonly ICustomerApiClient _customerApiClient;

    /// <summary>
    /// Initialises a new instance of the <see cref="CustomerApiHealthCheck" /> class.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger{CustomerApiHealthCheck}"/></param>
    /// <param name="customerApiClient">An instance of <see cref="ICustomerApiClient"/></param>
    public CustomerApiHealthCheck(
        ILogger<CustomerApiHealthCheck> logger,
        ICustomerApiClient customerApiClient)
    {
        _logger = logger;
        _customerApiClient = customerApiClient;
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
            await _customerApiClient.PingAsync();

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            string message = "Failed customer API health check!";

            _logger.LogError(ex, message);

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                description: message,
                exception: ex,
                data: null);
        }
    }
}