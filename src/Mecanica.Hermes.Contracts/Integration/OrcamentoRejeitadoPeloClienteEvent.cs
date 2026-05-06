using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de Cadastros quando o cliente rejeita o orçamento via webhook HMAC.
///     Consumido pela API de OS para promover a ordem para <c>Rejeitada</c>.
/// </summary>
[EntityName("orcamento.rejeitado-pelo-cliente.v1")]
public sealed record OrcamentoRejeitadoPeloClienteEvent(
    Guid OrdemDeServicoId,
    DateTime RejeitadoEm,
    string? Motivo);
