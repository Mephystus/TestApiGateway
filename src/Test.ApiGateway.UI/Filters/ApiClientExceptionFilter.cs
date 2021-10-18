// -------------------------------------------------------------------------------------
//  <copyright file="ApiClientExceptionFilter.cs" company="The AA (Ireland)">
//    Copyright (c) The AA (Ireland). All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------------------

namespace Test.ApiGateway.UI.Filters;

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SharedLibrary.Api.Client.Exceptions;
using SharedLibrary.Models.Models.Error;
using SharedLibrary.Models.Models.Validation;

/// <summary>
/// Exception filter to intercept <see cref="ApiClientException"/>
/// </summary>
public class ApiClientExceptionFilter : IAsyncExceptionFilter
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<ApiClientExceptionFilter> _logger;

    /// <summary>
    /// Initialises a new instance of the <see cref="ApiClientExceptionFilter" /> class.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger{ApiClientExceptionFilter}"/></param>
    public ApiClientExceptionFilter(ILogger<ApiClientExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called after an action has thrown an System.Exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    /// <returns>A Task that on completion indicates the filter has executed.</returns>
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (!context.ExceptionHandled && context.Exception is ApiClientException apiClientException) 
        {
            _logger.LogError(apiClientException, "API Client exception!");

            var responseString = await apiClientException.GetResponseContentAsync();

            switch (apiClientException.HttpStatusCode)
            {
                case HttpStatusCode.BadRequest:
                    var validationResponse = JsonConvert.DeserializeObject<ValidationResponse>(responseString);

                    context.Result = new ObjectResult(validationResponse)
                    {
                        StatusCode = (int)apiClientException.HttpStatusCode
                    };

                    break;

                case HttpStatusCode.NotFound:
                case HttpStatusCode.InternalServerError:
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseString);

                    context.Result = new ObjectResult(errorResponse)
                    {
                        StatusCode = (int)apiClientException.HttpStatusCode
                    };

                    break;

                default:
                    var anotherResponse = new ErrorResponse
                    {
                        StatusCode = (int)apiClientException.HttpStatusCode
                    };

                    anotherResponse.Details.Add(new ErrorDetail
                    {
                        Message = "An error occurred, please try again later."
                    });

                    context.Result = new ObjectResult(anotherResponse)
                    {
                        StatusCode = (int)apiClientException.HttpStatusCode
                    };
                    
                    break;
            }

            context.ExceptionHandled = true;
        }         
    }
}