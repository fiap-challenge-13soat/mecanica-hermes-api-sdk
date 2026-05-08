using Mecanica.Hermes.Shared.Core.Domain;

namespace Mecanica.Hermes.Shared.Tests.Core.Domain;

public class BaseDomainTests
{
    private sealed class TestEntity : BaseDomain
    {
        public TestEntity() { }
        public TestEntity(Guid id) : base(id) { }

        public void MarkAsUpdatedExposed() => MarkAsUpdated();
        public void RestaurarBaseExposed(Guid id, DateTime createdAt, DateTime? updatedAt)
            => RestaurarBase(id, createdAt, updatedAt);
    }

    [Fact]
    public void Ctor_SemId_GeraIdVazio_ECreatedAtPreenchido()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var entity = new TestEntity();

        entity.Id.Should().Be(Guid.Empty);
        entity.CreatedAt.Should().BeOnOrAfter(before);
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Ctor_ComId_AtribuiId()
    {
        var id = Guid.NewGuid();

        var entity = new TestEntity(id);

        entity.Id.Should().Be(id);
    }

    [Fact]
    public void MarkAsUpdated_PreencheUpdatedAt()
    {
        var entity = new TestEntity();
        entity.UpdatedAt.Should().BeNull();

        entity.MarkAsUpdatedExposed();

        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeOnOrAfter(entity.CreatedAt);
    }

    [Fact]
    public void RestaurarBase_SubstituiTodosOsCampos()
    {
        var entity = new TestEntity();
        var newId = Guid.NewGuid();
        var newCreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var newUpdatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        entity.RestaurarBaseExposed(newId, newCreatedAt, newUpdatedAt);

        entity.Id.Should().Be(newId);
        entity.CreatedAt.Should().Be(newCreatedAt);
        entity.UpdatedAt.Should().Be(newUpdatedAt);
    }

    [Fact]
    public void RestaurarBase_AceitaUpdatedAtNulo()
    {
        var entity = new TestEntity();
        entity.MarkAsUpdatedExposed();

        entity.RestaurarBaseExposed(Guid.NewGuid(), DateTime.UtcNow, null);

        entity.UpdatedAt.Should().BeNull();
    }
}
