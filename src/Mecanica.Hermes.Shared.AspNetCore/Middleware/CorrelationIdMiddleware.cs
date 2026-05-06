using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Mecanica.Hermes.Shared.AspNetCore.Middleware;

/// <summary>
///     Lê (ou gera) o header <c>X-Correlation-ID</c> em cada request, retorna o mesmo valor
///     no response e empurra a propriedade <c>CorrelationId</c> no Serilog LogContext durante
///     toda a duração da requisição.
/// </summary>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
