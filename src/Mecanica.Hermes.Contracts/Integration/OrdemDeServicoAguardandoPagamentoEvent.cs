using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de OS quando uma ordem transita para o status <c>AguardandoPagamento</c>.
///     Consumido pela API de Pagamentos para iniciar a geração do link no Mercado Pago.
/// </summary>
[EntityName("ordem-de-servico.aguardando-pagamento.v1")]
public sealed record OrdemDeServicoAguardandoPagamentoEvent(
    Guid OrdemDeServicoId,
    Guid ClienteId,
    decimal Valor);
