using System.Net;
using Newtonsoft.Json;
using TenderInfoAPI.Exceptions;

namespace TenderInfoAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var errorDetails = new ErrorDetails();

        if (exception is ApiException apiException)
        {
            context.Response.StatusCode = apiException.StatusCode;
            errorDetails.StatusCode = apiException.StatusCode;
            errorDetails.Message = apiException.Message;
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            errorDetails.StatusCode = context.Response.StatusCode;
            errorDetails.Message = "Internal Server Error. Please try again later.";
        }

        var response = JsonConvert.SerializeObject(errorDetails);
        return context.Response.WriteAsync(response);
    }
}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
