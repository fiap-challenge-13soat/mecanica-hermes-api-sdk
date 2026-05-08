using Mecanica.Hermes.Shared.Application.Behaviors;
using Mecanica.Hermes.Shared.Core.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mecanica.Hermes.Shared.Tests.Application;

public class LoggingBehaviorTests
{
    private sealed record FakeRequest(string Nome) : IRequest<Result>;

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
    }

    [Fact]
    public async Task Handle_RetornaResponseDoNext()
    {
        var logger = new CapturingLogger<LoggingBehavior<FakeRequest, Result>>();
        var sut = new LoggingBehavior<FakeRequest, Result>(logger);
        var expected = Result.Ok();

        var actual = await sut.Handle(
            new FakeRequest("x"),
            _ => Task.FromResult(expected),
            CancellationToken.None);

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_Sucesso_LogaInformationEDebug()
    {
        var logger = new CapturingLogger<LoggingBehavior<FakeRequest, Result>>();
        var sut = new LoggingBehavior<FakeRequest, Result>(logger);

        await sut.Handle(
            new FakeRequest("x"),
            _ => Task.FromResult(Result.Ok()),
            CancellationToken.None);

        logger.Entries.Should().Contain(e => e.Level == LogLevel.Information);
        logger.Entries.Should().Contain(e =>
            e.Level == LogLevel.Debug && e.Message.Contains("Success", StringComparison.Ordinal));
        logger.Entries.Should().NotContain(e => e.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task Handle_Falha_LogaWarningComErros()
    {
        var logger = new CapturingLogger<LoggingBehavior<FakeRequest, Result>>();
        var sut = new LoggingBehavior<FakeRequest, Result>(logger);

        await sut.Handle(
            new FakeRequest("x"),
            _ => Task.FromResult(Result.BadRequest("invalido")),
            CancellationToken.None);

        logger.Entries.Should().Contain(e => e.Level == LogLevel.Information);
        logger.Entries.Should().Contain(e =>
            e.Level == LogLevel.Warning && e.Message.Contains("Failure", StringComparison.Ordinal));
        logger.Entries.Should().NotContain(e => e.Level == LogLevel.Debug);
    }

    [Fact]
    public async Task Handle_LogaNomeDoRequestType()
    {
        var logger = new CapturingLogger<LoggingBehavior<FakeRequest, Result>>();
        var sut = new LoggingBehavior<FakeRequest, Result>(logger);

        await sut.Handle(
            new FakeRequest("x"),
            _ => Task.FromResult(Result.Ok()),
            CancellationToken.None);

        logger.Entries.Should().Contain(e => e.Message.Contains(nameof(FakeRequest), StringComparison.Ordinal));
    }
}
