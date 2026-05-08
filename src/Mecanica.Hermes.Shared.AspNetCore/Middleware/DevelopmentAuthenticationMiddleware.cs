using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Mecanica.Hermes.Shared.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Mecanica.Hermes.Shared.AspNetCore.Middleware;

/// <summary>
///     Bypass de autenticação para ambientes de Development e Testing. Injeta um ClaimsPrincipal
///     com os scopes configurados em <see cref="DevelopmentAuthenticationOptions" />.
///     <para>
///         Guard defensivo: lança em produção mesmo se algum operador encadear o middleware por engano.
///     </para>
/// </summary>
public class DevelopmentAuthenticationMiddleware
{
    private readonly IHostEnvironment _environment;
    private readonly RequestDelegate _next;
    private readonly DevelopmentAuthenticationOptions _options;

    public DevelopmentAuthenticationMiddleware(
        RequestDelegate next,
        IHostEnvironment environment,
        DevelopmentAuthenticationOptions options)
    {
        if (!environment.IsDevelopment() && environment.EnvironmentName != EnvironmentNames.Testing)
            throw new InvalidOperationException(
                $"DevelopmentAuthenticationMiddleware foi instanciado em ambiente '{environment.EnvironmentName}'. " +
                "Este middleware concede acesso irrestrito e NUNCA deve ser usado em produção.");

        _next = next;
        _environment = environment;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if ((_environment.IsDevelopment() || _environment.EnvironmentName == EnvironmentNames.Testing) &&
            context.User.Identity?.IsAuthenticated != true)
        {
            var scopes = string.Join(' ', _options.Scopes);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "test-user-id"),
                new(ClaimTypes.Name, "Test User"),
                new(ClaimTypes.Email, "test@example.com"),
                new("scope", scopes)
            };

            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}

/// <summary>Opções injetadas para o <see cref="DevelopmentAuthenticationMiddleware" />.</summary>
public sealed class DevelopmentAuthenticationOptions
{
    /// <summary>Scopes injetados no claim <c>scope</c> do usuário fake.</summary>
    public IReadOnlyList<string> Scopes { get; init; } = ["mechermes-admin/admin", "mechermes-admin/client"];
}

[ExcludeFromCodeCoverage(Justification = "Wrapper de UseMiddleware — pipeline ASP.NET, validado em integração.")]
public static class DevelopmentAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseDevelopmentAuthentication(
        this IApplicationBuilder builder,
        DevelopmentAuthenticationOptions? options = null)
    {
        return builder.UseMiddleware<DevelopmentAuthenticationMiddleware>(
            options ?? new DevelopmentAuthenticationOptions());
    }
}
