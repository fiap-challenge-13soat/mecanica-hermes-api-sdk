using System.Diagnostics.CodeAnalysis;
using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de Cadastros quando o cliente aprova o orçamento via webhook HMAC.
///     Consumido pela API de OS para promover a ordem para <c>EmExecucao</c>.
/// </summary>
[EntityName("orcamento.aprovado-pelo-cliente.v1")]
[ExcludeFromCodeCoverage(Justification = "Record DTO — auto-props; EntityName validado em IntegrationEventNamesTests.")]
public sealed record OrcamentoAprovadoPeloClienteEvent(Guid OrdemDeServicoId, DateTime AprovadoEm);
