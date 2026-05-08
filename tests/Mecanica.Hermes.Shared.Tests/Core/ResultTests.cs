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

    [Fact]
    public void NotFound_SemMensagem_RetornaStatus404SemErros()
    {
        var result = Result.NotFound();

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Errors.Should().BeEmpty();
    }

    private static readonly string[] BadRequestSampleErrors = ["e1", "e2", "e3"];
    private static readonly string[] GenericBadRequestSampleErrors = ["a", "b"];

    [Fact]
    public void BadRequest_ComLista_AdicionaTodosOsErros()
    {
        var result = Result.BadRequest(BadRequestSampleErrors);

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().ContainInOrder("e1", "e2", "e3");
    }

    [Fact]
    public void Generic_OkSemValor_RetornaStatus200ComDataPadrao()
    {
        var result = Result<int>.Ok();

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().Be(0);
    }

    [Fact]
    public void Generic_Forbidden_RetornaStatus403()
    {
        var result = Result<int>.Forbidden();

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public void Generic_Unauthorized_RetornaStatus401()
    {
        var result = Result<int>.Unauthorized();

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void Generic_NoContent_RetornaStatus204()
    {
        var result = Result<int>.NoContent();

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public void Generic_NotFoundSemMensagem_RetornaStatus404()
    {
        var result = Result<int>.NotFound();

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Generic_Conflict_RetornaStatus409ComErro()
    {
        var result = Result<int>.Conflict("já existe");

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Errors.Should().ContainSingle().Which.Should().Be("já existe");
    }

    [Fact]
    public void Generic_BadRequestComMensagem_RetornaStatus400ComErro()
    {
        var result = Result<int>.BadRequest("invalido");

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().ContainSingle().Which.Should().Be("invalido");
    }

    [Fact]
    public void Generic_BadRequestComLista_AdicionaTodosOsErros()
    {
        var result = Result<int>.BadRequest(GenericBadRequestSampleErrors);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().ContainInOrder("a", "b");
    }

    [Fact]
    public void Generic_InternalServerError_RetornaStatus500ComErro()
    {
        var result = Result<int>.InternalServerError("boom");

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.Errors.Should().ContainSingle().Which.Should().Be("boom");
    }

    [Fact]
    public void IsSuccess_FalseQuandoErrorListPreenchida_AindaQueStatus2xx()
    {
        var result = new Result<int>(HttpStatusCode.OK);
        result.Errors.Add("erro tardio");

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }
}
