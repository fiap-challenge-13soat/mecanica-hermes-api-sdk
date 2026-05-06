using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de Pagamentos quando o Mercado Pago recusa, cancela ou expira o pagamento.
///     Consumido pela API de OS, que reverte a ordem para <c>ManutencaoFinalizada</c>.
/// </summary>
[EntityName("pagamento.recusado.v1")]
public sealed record PagamentoRecusadoEvent(
    Guid OrdemDeServicoId,
    Guid PagamentoId,
    string Motivo,
    DateTime RecusadoEm);
