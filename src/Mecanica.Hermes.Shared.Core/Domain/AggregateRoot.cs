namespace Mecanica.Hermes.Shared.Core.Domain;

/// <summary>
///     Raiz de agregado: <see cref="BaseDomain" /> + lista de domain events emitidos
///     durante a vida do agregado, coletados pelo Unit of Work após commit.
/// </summary>
public abstract class AggregateRoot : BaseDomain
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
