// -------------------------------------------------------------------------------------
//  <copyright file="CustomerPoliciesResponse.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

namespace Test.ApiGateway.UI.Models;

/// <summary>
/// Defines the customer policies response.
/// </summary>
public class CustomerPoliciesResponse
{
    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the policy numbers.
    /// </summary>
    public List<string> PolicyNumbers { get; set; } = new List<string>();
}