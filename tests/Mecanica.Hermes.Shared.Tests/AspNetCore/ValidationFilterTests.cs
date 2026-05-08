using FluentValidation;
using FluentValidation.Results;
using Mecanica.Hermes.Shared.AspNetCore.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Mecanica.Hermes.Shared.Tests.AspNetCore;

public class ValidationFilterTests
{
    public sealed record FakeRequest(string Nome);

    [Fact]
    public async Task Invoke_ArgumentoTipadoAusente_PassaParaProximo()
    {
        var validator = new Mock<IValidator<FakeRequest>>(MockBehavior.Strict);
        var sut = new ValidationFilter<FakeRequest>(validator.Object);
        var ctx = EndpointFilterInvocationContext.Create(new DefaultHttpContext(), "texto-irrelevante", 42);
        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>("ok");

        var result = await sut.InvokeAsync(ctx, next);

        result.Should().Be("ok");
        validator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Invoke_ArgumentoValido_ChamaProximo()
    {
        var validator = new Mock<IValidator<FakeRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<FakeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var sut = new ValidationFilter<FakeRequest>(validator.Object);
        var ctx = EndpointFilterInvocationContext.Create(new DefaultHttpContext(), new FakeRequest("ok"));
        var nextCalls = 0;
        EndpointFilterDelegate next = _ =>
        {
            nextCalls++;
            return ValueTask.FromResult<object?>("ok");
        };

        var result = await sut.InvokeAsync(ctx, next);

        nextCalls.Should().Be(1);
        result.Should().Be("ok");
        validator.Verify(v => v.ValidateAsync(It.IsAny<FakeRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Invoke_ArgumentoInvalido_RetornaProblemDetailsComErros()
    {
        var failures = new List<ValidationFailure>
        {
            new("Nome", "Nome é obrigatório"),
            new("Nome", "Nome deve ter ao menos 3 caracteres"),
        };
        var validationResult = new ValidationResult(failures);
        var validator = new Mock<IValidator<FakeRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<FakeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        var sut = new ValidationFilter<FakeRequest>(validator.Object);
        var ctx = EndpointFilterInvocationContext.Create(new DefaultHttpContext(), new FakeRequest(""));
        var nextCalled = false;
        EndpointFilterDelegate next = _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>("ok");
        };

        var result = await sut.InvokeAsync(ctx, next);

        nextCalled.Should().BeFalse();
        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem.ProblemDetails.Title.Should().Be("Requisição Inválida.");
        problem.ProblemDetails.Extensions.Should().ContainKey("errors");
        var errors = problem.ProblemDetails.Extensions["errors"] as IReadOnlyCollection<string>;
        errors.Should().NotBeNull();
        errors!.Should().Contain("Nome é obrigatório");
        errors.Should().Contain("Nome deve ter ao menos 3 caracteres");
    }
}
