using System.Net;
using Mecanica.Hermes.Shared.Application.Messaging;
using Mecanica.Hermes.Shared.Core.Results;
using Moq;

namespace Mecanica.Hermes.Shared.Tests.Application;

public class AsyncCommandHandlerTests
{
    private sealed record FakeMessage(string Conteudo);

    private sealed record FakeCommand(string Conteudo) : IAsyncCommand
    {
        public object ToMessage() => new FakeMessage(Conteudo);
    }

    [Fact]
    public async Task Handle_PublicaMensagemRetornadaPeloCommand()
    {
        object? captured = null;
        var publisher = new Mock<IAsyncMessagePublisher>();
        publisher
            .Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((msg, _) => captured = msg)
            .Returns(Task.CompletedTask);
        var sut = new AsyncCommandHandler<FakeCommand>(publisher.Object);

        await sut.Handle(new FakeCommand("payload"), CancellationToken.None);

        captured.Should().BeOfType<FakeMessage>().Which.Conteudo.Should().Be("payload");
        publisher.Verify(p => p.Publish(It.IsAny<object>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_RetornaResultAccepted()
    {
        var publisher = new Mock<IAsyncMessagePublisher>();
        var sut = new AsyncCommandHandler<FakeCommand>(publisher.Object);

        var result = await sut.Handle(new FakeCommand("x"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Handle_PropagaCancellationToken()
    {
        var publisher = new Mock<IAsyncMessagePublisher>();
        var sut = new AsyncCommandHandler<FakeCommand>(publisher.Object);
        using var cts = new CancellationTokenSource();

        await sut.Handle(new FakeCommand("x"), cts.Token);

        publisher.Verify(p => p.Publish(It.IsAny<object>(), cts.Token), Times.Once);
    }
}
