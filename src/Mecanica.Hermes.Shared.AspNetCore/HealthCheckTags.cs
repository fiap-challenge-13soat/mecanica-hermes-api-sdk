using System.Diagnostics.CodeAnalysis;

namespace Mecanica.Hermes.Shared.AspNetCore;

/// <summary>
///     Tags compartilhadas para health checks ASP.NET. Filtros como
///     <c>Predicate = hc =&gt; hc.Tags.Contains(HealthCheckTags.Ready)</c> dependem desses valores.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Constantes — sem lógica testável.")]
public static class HealthCheckTags
{
    public const string Ready = "ready";
    public const string Database = "database";
    public const string Messaging = "messaging";
    public const string Saga = "saga";
}
