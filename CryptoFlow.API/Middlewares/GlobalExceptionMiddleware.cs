using System.Net;
using System.Text.Json;
using CryptoFlow.Application.Exceptions;

namespace CryptoFlow.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var message = "Sunucu kaynaklı hata oluştu lütfen yetkili birine bildirin";
        var statusCode = (int)HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case (NotFoundException):
                statusCode = (int)HttpStatusCode.NotFound;
                message = exception.Message;
                _logger.LogWarning(message);
                break;
            case(WrongInputException):
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                _logger.LogWarning(message);
                break;
            case OperationCanceledException:
                _logger.LogInformation("Kullanıcı bağlantıyı kopardı (Stream iptal edildi).");
                break;
            default:
                _logger.LogError(exception,  exception.Message);
                break;
        }
        if (context.Response.HasStarted)
        {
            return Task.CompletedTask;
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var response = new
        {
            StatusCode = statusCode,
            Message = message
        };
        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);

    }
    
    
}