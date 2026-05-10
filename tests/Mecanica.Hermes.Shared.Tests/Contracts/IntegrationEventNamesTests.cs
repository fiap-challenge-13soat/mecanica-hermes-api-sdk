using MassTransit;
using Mecanica.Hermes.Contracts.Integration;

namespace Mecanica.Hermes.Shared.Tests.Contracts;

/// <summary>
///     Garante que os 8 contratos cross-service têm o EntityName canônico esperado.
///     Quebrar esses nomes é breaking change e exige bump major do pacote Contracts.
/// </summary>
public class IntegrationEventNamesTests
{
    [Theory]
    [InlineData(typeof(OrdemDeServicoAguardandoAprovacaoEvent), "ordem-de-servico.aguardando-aprovacao.v1")]
    [InlineData(typeof(OrdemDeServicoAguardandoPagamentoEvent), "ordem-de-servico.aguardando-pagamento.v1")]
    [InlineData(typeof(OrdemDeServicoCanceladaEvent), "ordem-de-servico.cancelada.v1")]
    [InlineData(typeof(OrcamentoAprovadoPeloClienteEvent), "orcamento.aprovado-pelo-cliente.v1")]
    [InlineData(typeof(OrcamentoRejeitadoPeloClienteEvent), "orcamento.rejeitado-pelo-cliente.v1")]
    [InlineData(typeof(PagamentoConfirmadoEvent), "pagamento.confirmado.v1")]
    [InlineData(typeof(PagamentoRecusadoEvent), "pagamento.recusado.v1")]
    [InlineData(typeof(LinkPagamentoGeradoEvent), "pagamento.link-gerado.v1")]
    public void Contrato_Tem_EntityName_Esperado(Type contractType, string expectedEntityName)
    {
        var attribute = contractType.GetCustomAttributes(typeof(EntityNameAttribute), false)
            .OfType<EntityNameAttribute>()
            .FirstOrDefault();

        attribute.Should().NotBeNull(
            $"{contractType.Name} deve ter [EntityName] decorator para roteamento canônico cross-service");
        attribute!.EntityName.Should().Be(expectedEntityName,
            $"O EntityName de {contractType.Name} é parte do contrato e não pode mudar sem bump major");
    }
}
