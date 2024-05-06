using Forge.TransactionHandler.ServiceConfiguration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;


namespace Forge.TransactionHandler;

public sealed class TransactionHandler(
    RequestDelegate next,
    ILogger<TransactionHandler>? logger = null
    )
{
    readonly RequestDelegate _next = next;
    readonly ILogger<TransactionHandler>? _logger = logger;

    internal static TransactionHandlerConfig? Configuration { get; set; }
    public async Task InvokeAsync(
        HttpContext httpContext,
        IEnumerable<ITransactionGroup> groups
        )
    {

        await _next(httpContext);

        var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;

        if (endpoint is null 
            || endpoint.Metadata.Any(m => m is DisableTransactionHandlerAttribute))
        {
            return;
        }

        var requestAborted = httpContext.RequestAborted;

        foreach (var group in groups)
        {
            var groupName = group.GetName();

            if (!Configuration?[groupName].ShouldSave(httpContext) ?? false) continue;

            try
            {
                await group.SaveChangesAsync(requestAborted);

                if(!(Configuration?.IsLoggingDisabled ?? false)
                    && (_logger?.IsEnabled(LogLevel.Information) ?? false)) 
                {
                    _logger.LogInformation("Transaction group {name} executed succesfully", groupName);
                }

            }
            catch
            {

                if (!(Configuration?.IsLoggingDisabled ?? false)
                    && (_logger?.IsEnabled(LogLevel.Critical) ?? false))
                {
                    _logger.LogCritical("Transaction group {name} executed unsuccesfully", groupName);
                }

                throw;
            }
        }

    }
}
