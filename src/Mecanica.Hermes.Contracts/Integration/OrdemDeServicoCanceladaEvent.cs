using System.Diagnostics.CodeAnalysis;
using MassTransit;

namespace Mecanica.Hermes.Contracts.Integration;

/// <summary>
///     Publicado pela API de OS quando uma Ordem de Serviço é cancelada (operador ou regra
///     de negócio). Consumido pela API de Pagamentos para recusar/cancelar pagamentos
///     pendentes vinculados à OS, evitando que o cliente continue podendo pagar uma OS já
///     cancelada e que o polling do Mercado Pago siga ativo desnecessariamente.
/// </summary>
[EntityName("ordem-de-servico.cancelada.v1")]
[ExcludeFromCodeCoverage(Justification = "Record DTO — auto-props; EntityName validado em IntegrationEventNamesTests.")]
public sealed record OrdemDeServicoCanceladaEvent(
    Guid OrdemDeServicoId,
    string StatusAnterior,
    DateTime OcorridoEm);
