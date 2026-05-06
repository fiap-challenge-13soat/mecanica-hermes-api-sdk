using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Shared.AspNetCore.Presenter;

/// <summary>
///     Extensão do <see cref="ProblemDetails" /> RFC 7807 com lista de erros estruturada
///     para responder validações e falhas de domínio em formato consistente.
/// </summary>
public sealed class ApiProblemDetails : ProblemDetails
{
    public List<string> Errors { get; init; } = [];
}
