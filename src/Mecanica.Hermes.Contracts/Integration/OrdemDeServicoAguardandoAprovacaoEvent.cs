using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de OS quando uma ordem transita para o status <c>AguardandoAprovacao</c>.
///     Consumido pela API de Cadastros para enfileirar o envio de e-mail de orçamento.
/// </summary>
[EntityName("ordem-de-servico.aguardando-aprovacao.v1")]
public sealed record OrdemDeServicoAguardandoAprovacaoEvent(Guid OrdemDeServicoId);
