using Microsoft.AspNetCore.Builder;

namespace Mecanica.Hermes.Shared.AspNetCore.Middleware;

public static class MiddlewareExtensions
{
    /// <summary>Liga o <see cref="CorrelationIdMiddleware" /> no pipeline.</summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
