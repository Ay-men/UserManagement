using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace HomeManagement.AuthService.Api.Middlewares
{
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected error occurred.");
        await HandleExceptionAsync(context, ex);
      }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      var response = context.Response;
      response.ContentType = "application/json";
      response.StatusCode = (int)HttpStatusCode.InternalServerError;

      var result = JsonSerializer.Serialize(new { error = exception.Message });
      return response.WriteAsync(result);
    }
  }
}
