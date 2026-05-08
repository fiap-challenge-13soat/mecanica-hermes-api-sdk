using Mecanica.Hermes.Shared.Core.Domain;

namespace Mecanica.Hermes.Shared.Tests.Core.Domain;

public class DomainClockTests
{
    private sealed class FixedTimeProvider(DateTimeOffset instant) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => instant;
    }

    [Fact]
    public void UtcNow_SemOverride_UsaSystemTimeProvider()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var now = DomainClock.UtcNow;

        now.Should().BeOnOrAfter(before);
        now.Should().BeOnOrBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Override_TrocaProviderEnquantoEscopoVivo()
    {
        var fixedInstant = new DateTimeOffset(2025, 3, 14, 9, 26, 53, TimeSpan.Zero);

        using (DomainClock.Override(new FixedTimeProvider(fixedInstant)))
        {
            DomainClock.UtcNow.Should().Be(fixedInstant.UtcDateTime);
        }
    }

    [Fact]
    public void Override_DescartadoRestauraSystemTimeProvider()
    {
        var fixedInstant = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

        using (DomainClock.Override(new FixedTimeProvider(fixedInstant)))
        {
            DomainClock.UtcNow.Should().Be(fixedInstant.UtcDateTime);
        }

        DomainClock.UtcNow.Year.Should().BeGreaterThan(2020);
    }
}
