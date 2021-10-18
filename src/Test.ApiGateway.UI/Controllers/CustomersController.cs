// -------------------------------------------------------------------------------------
//  <copyright file="CustomersController.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

namespace Test.ApiGateway.UI.Controllers;

using Customer.Api.Client.Interfaces;
using Customer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Policy.Api.Client.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Test.ApiGateway.UI.Models;

/// <summary>
/// The customers controller.
/// </summary>
[Route("gateway/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    /// <summary>
    /// The customer API client.
    /// </summary>
    private readonly ICustomerApiClient _customerApiClient;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<CustomersController> _logger;

    /// <summary>
    /// The policy API client.
    /// </summary>
    private readonly IPolicyApiClient _policyApiClient;

    /// <summary>
    /// Initialises a new instance of the <see cref="CustomersController"/> class.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger{CustomersController}"/></param>
    /// <param name="customerApiClient">An instance of <see cref="ICustomerApiClient"/></param>
    /// <param name="policyApiClient">An instance of <see cref="IPolicyApiClient"/></param>
    public CustomersController(
        ILogger<CustomersController> logger,
        ICustomerApiClient customerApiClient,
        IPolicyApiClient policyApiClient)
    {
        _logger = logger;
        _customerApiClient = customerApiClient;
        _policyApiClient = policyApiClient;
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="customerRequest">The customer request.</param>
    /// <returns>The customer.</returns>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created, "The customer was created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Failed validation")]
    public async Task<IActionResult> CreateCustomerAsync(CustomerRequest customerRequest)
    {
        _logger.LogInformation("Input: {@customerRequest}", customerRequest);

        await _customerApiClient.CreateCustomerAsync(customerRequest);

        return CreatedAtAction(nameof(GetCustomerAsync),
                                new { id = customerRequest.Id },
                                customerRequest);
    }

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="id">The customer Id.</param>
    /// <returns>The customer.</returns>
    [HttpDelete("{id:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "The customer deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer not found.")]
    public async Task<IActionResult> DeleteCustomerAsync(Guid id)
    {
        _logger.LogInformation("Input: {id}", id);

        await _customerApiClient.DeleteCustomerAsync(id);

        return Ok();
    }

    /// <summary>
    /// Gets the customer.
    /// </summary>
    /// <param name="id">The customer Id.</param>
    /// <returns>The customer.</returns>
    [HttpGet("{id:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "The customer", typeof(CustomerResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer not found.")]
    public async Task<IActionResult> GetCustomerAsync(Guid id)
    {
        _logger.LogInformation("Input: {id}", id);

        var customer = await _customerApiClient.GetCustomerAsync(id);

        return Ok(customer);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="customerRequest">The customer request.</param>
    /// <returns>The customer.</returns>
    [HttpPut]
    [SwaggerResponse(StatusCodes.Status200OK, "The updated customer", typeof(CustomerResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Failed validation")]
    public async Task<IActionResult> UpdateCustomerAsync(CustomerRequest customerRequest)
    {
        _logger.LogInformation("Input: {@customerRequest}", customerRequest);

        //// TESTING SEQUENTIAL

        await _customerApiClient.UpdateCustomerAsync(customerRequest);

        var customer = await _customerApiClient.GetCustomerAsync(customerRequest.Id);

        return Ok(customer);
    }

    /// <summary>
    /// Gets the customer's policies.
    /// </summary>
    /// <param name="id">The customer Id.</param>
    /// <returns>The policies.</returns>
    [HttpGet("{id:guid}/policies")]
    [SwaggerResponse(StatusCodes.Status200OK, "The customer policies", typeof(CustomerPoliciesResponse))]
    public async Task<IActionResult> GetCustomerPoliciesAsync(Guid id)
    {
        _logger.LogInformation("Customer ID: {id}", id);

        //// TESTING PARALELL
        
        var customerTask = _customerApiClient.GetCustomerAsync(id);

        var customerPoliciesTask = _policyApiClient.GetCustomerPoliciesAsync(id);

        var customer = await customerTask;
        var customerPolicies = await customerPoliciesTask;

        var response = new CustomerPoliciesResponse
        {
            FullName = $"{customer.FirstName} {customer.LastName}",
            PolicyNumbers = customerPolicies.Select(x => x.PolicyNumber).ToList()
        };

        return Ok(response);
    }
}
