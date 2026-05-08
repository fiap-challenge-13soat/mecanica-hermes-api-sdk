using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;

namespace Mecanica.Hermes.Shared.AspNetCore.Middleware;

[ExcludeFromCodeCoverage(Justification = "Wrapper de UseMiddleware — pipeline ASP.NET, validado em integração.")]
public static class MiddlewareExtensions
{
    /// <summary>Liga o <see cref="CorrelationIdMiddleware" /> no pipeline.</summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
