using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Response.Headers.Add("X-Correlation-ID", correlationId);
        Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId);

        await _next(context);
    }
}
