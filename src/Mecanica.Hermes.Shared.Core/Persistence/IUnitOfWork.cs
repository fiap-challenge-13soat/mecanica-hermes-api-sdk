using Mecanica.Hermes.Shared.Core.Domain;

namespace Mecanica.Hermes.Shared.Core.Persistence;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken);
    void CollectEventsFrom(AggregateRoot aggregate);
}
