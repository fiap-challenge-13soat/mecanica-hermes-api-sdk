using System.Diagnostics.CodeAnalysis;

namespace Mecanica.Hermes.Shared.Core;

/// <summary>
///     Nomes de ambientes utilizados pelas aplicações Mecânica Hermes (alinhados com a convenção
///     <c>ASPNETCORE_ENVIRONMENT</c>). Centralizados no SDK para que qualquer
///     camada possa referenciá-los sem duplicação cross-repo.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Constantes — sem lógica testável.")]
public static class EnvironmentNames
{
    public const string Development = "Development";
    public const string Production = "Production";

    /// <summary>
    ///     Nome do ambiente usado pelos testes de integração / E2E. Quando ativo, autenticação
    ///     e logging Serilog são substituídos por adapters de teste.
    /// </summary>
    public const string Testing = "Testing";
}
