using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de Pagamentos quando o Mercado Pago confirma o pagamento (via webhook
///     ou polling). Consumido pela API de OS para promover a ordem para <c>PagamentoConfirmado</c>.
/// </summary>
[EntityName("pagamento.confirmado.v1")]
public sealed record PagamentoConfirmadoEvent(
    Guid OrdemDeServicoId,
    Guid PagamentoId,
    DateTime ConfirmadoEm);
