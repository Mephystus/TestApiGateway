// -------------------------------------------------------------------------------------
//  <copyright file="AuthConfiguration.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

namespace Test.ApiGateway.UI.Security;

/// <summary>
/// Defines the Auth configuration.
/// </summary>
public class AuthConfiguration
{
    /// <summary>
    /// Gets or sets the secret.
    /// </summary>
    public string Secret { get; set; } = default!;

    /// <summary>
    /// Gets or sets the Issuer.
    /// </summary>
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// Gets or sets the Audience.
    /// </summary>
    public string Audience { get; set; } = default!;
}