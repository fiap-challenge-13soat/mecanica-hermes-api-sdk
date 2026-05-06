namespace Mecanica.Hermes.Shared.Core.Domain;

/// <summary>
///     Associa um discriminador de nome estável a um tipo <see cref="IDomainEvent" />.
///     Usado para gravar o tipo do evento no Outbox ou em logs sem depender de
///     <c>AssemblyQualifiedName</c>, tornando o discriminador imune a refatorações
///     de namespace ou renomeação de assembly.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DomainEventNameAttribute(string name) : Attribute
{
    /// <summary>Nome lógico e imutável do evento — ex: <c>"ordem-de-servico.criado.v1"</c>.</summary>
    public string Name { get; } = name;
}
