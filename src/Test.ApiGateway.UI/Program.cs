// -------------------------------------------------------------------------------------
//  <copyright file="Program.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Customer.Api.Client;
using Serilog;
using SharedLibrary.Filters.Filters;
using Test.ApiGateway.UI.Filters;
using Test.ApiGateway.UI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

Log.Information("Starting up");

builder.Host.UseSerilog();

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});

// Add services to the container.
var services = builder.Services;

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

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Test.ApiGateway.UI", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.EnableAnnotations();
});

var appSettings = builder.Configuration.Get<AppSettings>();

services.AddCustomerHttpClient(appSettings.HttpClientSettingsDictionary);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test.ApiGateway.UI v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
