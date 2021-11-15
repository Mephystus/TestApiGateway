// -------------------------------------------------------------------------------------
//  <copyright file="Program.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

using System.Text.Json.Serialization;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using AspNetCoreRateLimit;
using Customer.Api.Client;
using Customer.Api.Client.Implementations;
using Customer.Api.Client.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Policy.Api.Client;
using Policy.Api.Client.Implementations;
using Policy.Api.Client.Interfaces;
using Serilog;
using SharedLibrary.Api.Client.Extensions;
using SharedLibrary.Api.Extensions;
using SharedLibrary.Filters.Filters;
using Test.ApiGateway.UI.Extensions;
using Test.ApiGateway.UI.Filters;
using Test.ApiGateway.UI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

Log.Information("Starting up");

builder.Host.UseSerilog();

builder.Host.UseMetricsWebTracking()
    .UseMetrics(options =>
    {
        options.EndpointOptions = endpointsOptions =>
        {
            endpointsOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
            endpointsOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
            endpointsOptions.EnvironmentInfoEndpointEnabled = false;
        };
    });

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
}).ConfigureAppConfiguration((hosting, config) =>
{
    var env = hosting.HostingEnvironment.EnvironmentName;
    config.AddJsonFile($"apiclientsettings.json");
    config.AddJsonFile($"apiclientsettings.{env}.json");
    config.AddJsonFile($"ratelimitsettings.json");
    config.AddJsonFile($"ratelimitsettings.{env}.json");
});

// Add services to the container.
var services = builder.Services;

services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

services.AddMetrics();

services.AddRateLimiting(builder.Configuration);

services.AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
        options.Filters.Add<UnhandledExceptionFilter>();
        options.Filters.Add<ApiClientExceptionFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        var enumConverter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var appSettings = builder.Configuration.Get<AppSettings>();

services.AddAuthentication(appSettings.AuthConfiguration);

services.AddSwagger();

services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(appSettings.HttpClientSettingsDictionary, nameof(ICustomerApiClient));
services.AddHttpClient<IPolicyApiClient, PolicyApiClient>(appSettings.HttpClientSettingsDictionary, nameof(IPolicyApiClient));

services.AddApiGatewayHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test.ApiGateway.UI v1"));
}

app.UseIpRateLimiting();

app.UseApiHealthChecks();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
