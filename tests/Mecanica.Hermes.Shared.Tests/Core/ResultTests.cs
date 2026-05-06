using System.Net;
using Mecanica.Hermes.Shared.Core.Results;

namespace Mecanica.Hermes.Shared.Tests.Core;

public class ResultTests
{
    [Fact]
    public void Ok_RetornaSucessoComStatus200()
    {
        var result = Result.Ok();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Accepted_RetornaSucessoComStatus202()
    {
        var result = Result.Accepted();

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void BadRequest_ComMensagem_AdicionaErro()
    {
        var result = Result.BadRequest("invalid");

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().ContainSingle().Which.Should().Be("invalid");
    }

    [Fact]
    public void NotFound_ComMensagem_AdicionaErro()
    {
        var result = Result.NotFound("missing");

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Errors.Should().ContainSingle().Which.Should().Be("missing");
    }

    [Fact]
    public void Generic_OkWithValue_PreservaValor()
    {
        var result = Result<int>.Ok(42);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(42);
    }

    [Fact]
    public void Generic_Created_RetornaStatus201ComValor()
    {
        var result = Result<string>.Created("abc");

        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Data.Should().Be("abc");
    }
}
