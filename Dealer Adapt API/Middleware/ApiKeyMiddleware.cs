using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Dealer_Adapt_API.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string APIKEYNAME = "ApiKey";
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        _logger.LogInformation("Checking for API key in headers...");

        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            _logger.LogWarning("Api Key was not provided.");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api Key was not provided.");
            return;
        }

        _logger.LogInformation($"Extracted API Key: {extractedApiKey}");

        var apiKey = configuration.GetValue<string>(APIKEYNAME);

        if (!apiKey.Equals(extractedApiKey))
        {
            _logger.LogWarning("Unauthorized client.");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}
   
