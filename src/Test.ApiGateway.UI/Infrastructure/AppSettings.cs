// -------------------------------------------------------------------------------------
//  <copyright file="AppSettings.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

namespace Test.ApiGateway.UI.Infrastructure;

using SharedLibrary.Api.Client.Configuration;

/// <summary>
/// Defines the configuration/settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the HTTP client settings.
    /// </summary>
    public Dictionary<string, HttpClientSettings> HttpClientSettingsDictionary { get; set; } = new Dictionary<string, HttpClientSettings>();
}
