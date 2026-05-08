using Mecanica.Hermes.Shared.AspNetCore.Middleware;
using Microsoft.AspNetCore.Http;

namespace Mecanica.Hermes.Shared.Tests.AspNetCore;

public class CorrelationIdMiddlewareTests
{
    private const string HeaderName = "X-Correlation-ID";

    [Fact]
    public async Task Invoke_HeaderAusente_GeraGuidEPropagaNoResponse()
    {
        var nextCalled = false;
        var middleware = new CorrelationIdMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var ctx = new DefaultHttpContext();

        await middleware.InvokeAsync(ctx);

        nextCalled.Should().BeTrue();
        var responseHeader = ctx.Response.Headers[HeaderName].ToString();
        responseHeader.Should().NotBeNullOrEmpty();
        Guid.TryParse(responseHeader, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Invoke_HeaderPresente_ReusaValorNoResponse()
    {
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers[HeaderName] = "abc-123";

        await middleware.InvokeAsync(ctx);

        ctx.Response.Headers[HeaderName].ToString().Should().Be("abc-123");
    }

    [Fact]
    public async Task Invoke_ChamaNextDelegate()
    {
        var calls = 0;
        var middleware = new CorrelationIdMiddleware(_ =>
        {
            calls++;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(new DefaultHttpContext());

        calls.Should().Be(1);
    }
}
