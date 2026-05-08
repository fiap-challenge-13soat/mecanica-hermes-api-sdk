using Mecanica.Hermes.Shared.Core.Domain;

namespace Mecanica.Hermes.Shared.Tests.Core.Domain;

public class AggregateRootTests
{
    private sealed record TestEvent(string Name) : IDomainEvent;

    private sealed class TestAggregate : AggregateRoot
    {
        public TestAggregate() { }
        public TestAggregate(Guid id) : base(id) { }

        public void Raise(IDomainEvent ev) => AddDomainEvent(ev);
    }

    [Fact]
    public void DomainEvents_NovoAgregado_VazioPorPadrao()
    {
        var aggregate = new TestAggregate();

        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_AdicionaNaListaPreservandoOrdem()
    {
        var aggregate = new TestAggregate();
        var first = new TestEvent("a");
        var second = new TestEvent("b");

        aggregate.Raise(first);
        aggregate.Raise(second);

        aggregate.DomainEvents.Should().HaveCount(2);
        aggregate.DomainEvents.Should().ContainInOrder(first, second);
    }

    [Fact]
    public void ClearDomainEvents_EsvaziaLista()
    {
        var aggregate = new TestAggregate();
        aggregate.Raise(new TestEvent("x"));

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Ctor_ComId_RepassaParaBase()
    {
        var id = Guid.NewGuid();

        var aggregate = new TestAggregate(id);

        aggregate.Id.Should().Be(id);
        aggregate.DomainEvents.Should().BeEmpty();
    }
}
