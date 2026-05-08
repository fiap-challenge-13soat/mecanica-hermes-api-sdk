using System.Security.Claims;
using Mecanica.Hermes.Shared.AspNetCore.Middleware;
using Mecanica.Hermes.Shared.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Mecanica.Hermes.Shared.Tests.AspNetCore;

public class DevelopmentAuthenticationMiddlewareTests
{
    private sealed class FakeEnv(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;
        public string ApplicationName { get; set; } = "tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact]
    public void Ctor_AmbienteProducao_Lanca()
    {
        var env = new FakeEnv("Production");

        Action act = () => _ = new DevelopmentAuthenticationMiddleware(
            _ => Task.CompletedTask,
            env,
            new DevelopmentAuthenticationOptions());

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Production*NUNCA*produção*");
    }

    [Fact]
    public async Task Invoke_Development_InjetaClaimsPrincipal()
    {
        var middleware = new DevelopmentAuthenticationMiddleware(
            _ => Task.CompletedTask,
            new FakeEnv(EnvironmentNames.Development),
            new DevelopmentAuthenticationOptions { Scopes = ["s1", "s2"] });
        var ctx = new DefaultHttpContext();

        await middleware.InvokeAsync(ctx);

        ctx.User.Identity!.IsAuthenticated.Should().BeTrue();
        ctx.User.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be("test-user-id");
        ctx.User.FindFirst(ClaimTypes.Email)!.Value.Should().Be("test@example.com");
        ctx.User.FindFirst("scope")!.Value.Should().Be("s1 s2");
    }

    [Fact]
    public async Task Invoke_Testing_InjetaClaimsPrincipal()
    {
        var middleware = new DevelopmentAuthenticationMiddleware(
            _ => Task.CompletedTask,
            new FakeEnv(EnvironmentNames.Testing),
            new DevelopmentAuthenticationOptions());
        var ctx = new DefaultHttpContext();

        await middleware.InvokeAsync(ctx);

        ctx.User.Identity!.IsAuthenticated.Should().BeTrue();
        ctx.User.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Test User");
    }

    [Fact]
    public async Task Invoke_QuandoJaAutenticado_NaoSubstituiUser()
    {
        var middleware = new DevelopmentAuthenticationMiddleware(
            _ => Task.CompletedTask,
            new FakeEnv(EnvironmentNames.Development),
            new DevelopmentAuthenticationOptions());
        var ctx = new DefaultHttpContext();
        var existing = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "real-user")],
            "real-auth"));
        ctx.User = existing;

        await middleware.InvokeAsync(ctx);

        ctx.User.Should().BeSameAs(existing);
        ctx.User.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be("real-user");
    }

    [Fact]
    public async Task Invoke_ChamaNextDelegate()
    {
        var calls = 0;
        var middleware = new DevelopmentAuthenticationMiddleware(
            _ =>
            {
                calls++;
                return Task.CompletedTask;
            },
            new FakeEnv(EnvironmentNames.Development),
            new DevelopmentAuthenticationOptions());

        await middleware.InvokeAsync(new DefaultHttpContext());

        calls.Should().Be(1);
    }

    [Fact]
    public void Options_PadraoTemDoisScopesAdmin()
    {
        var options = new DevelopmentAuthenticationOptions();

        options.Scopes.Should().HaveCount(2);
        options.Scopes.Should().Contain("mechermes-admin/admin");
        options.Scopes.Should().Contain("mechermes-admin/client");
    }
}
