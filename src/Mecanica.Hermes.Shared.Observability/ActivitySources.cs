using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Mecanica.Hermes.Shared.Observability;

/// <summary>
///     ActivitySource compartilhado para tracing OpenTelemetry. Cada API pode adicionar
///     ActivitySources específicos (ex.: <c>"MecanicaHermes.Pagamento.Application"</c>),
///     mas todos os serviços usam <see cref="Api" /> para spans no nível da requisição HTTP.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Constantes estáticas para tracing — valor testado em integração.")]
public static class ActivitySources
{
    public const string ApplicationName = "MecanicaHermes";

    public const string ApiName = ApplicationName + ".Api";

    public static readonly ActivitySource Api =
        new(ApiName, typeof(ActivitySources).Assembly.GetName().Version?.ToString() ?? "1.0");
}
