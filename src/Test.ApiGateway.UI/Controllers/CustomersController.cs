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
using Swashbuckle.AspNetCore.Annotations;

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
    /// Initialises a new instance of the <see cref="CustomersController"/> class.
    /// </summary>
    /// <param name="customerApiClient">An instance of <see cref="ICustomerApiClient"/></param>
    public CustomersController(ICustomerApiClient customerApiClient)
    {
        _customerApiClient = customerApiClient;
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
        var customer = await _customerApiClient.GetCustomerAsync(id);

        return Ok(customer);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="customerRequest">The customer request.</param>
    /// <returns>The customer.</returns>
    [HttpPut]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The customer was updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Failed validation")]
    public async Task<IActionResult> UpdateCustomerAsync(CustomerRequest customerRequest)
    {
        await _customerApiClient.UpdateCustomerAsync(customerRequest);

        return NoContent();
    }
}
