using System.Diagnostics.CodeAnalysis;
using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de Pagamentos quando o link no Mercado Pago é gerado e está pronto
///     para envio ao cliente. Consumido pela API de Cadastros para enfileirar o envio do e-mail.
/// </summary>
[EntityName("pagamento.link-gerado.v1")]
[ExcludeFromCodeCoverage(Justification = "Record DTO — auto-props; EntityName validado em IntegrationEventNamesTests.")]
public sealed record LinkPagamentoGeradoEvent(Guid PagamentoId);
