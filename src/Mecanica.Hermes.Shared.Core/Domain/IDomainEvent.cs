namespace Mecanica.Hermes.Shared.Core.Domain;

/// <summary>Marker interface para eventos de domínio. Eventos são in-process e podem ser
/// despachados após o commit do agregado para handlers MediatR ou para um Outbox.</summary>
public interface IDomainEvent
{
}
