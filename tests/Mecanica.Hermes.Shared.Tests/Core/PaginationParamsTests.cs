using Mecanica.Hermes.Shared.Core.Pagination;

namespace Mecanica.Hermes.Shared.Tests.Core;

public class PaginationParamsTests
{
    [Fact]
    public void DefaultValues_PageE1_PageSize10()
    {
        var p = new PaginationParams();
        p.Page.Should().Be(1);
        p.PageSize.Should().Be(10);
        p.Skip.Should().Be(0);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(5, 5)]
    public void Page_Coerce(int input, int expected)
    {
        new PaginationParams { Page = input }.Page.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-5, 10)]
    [InlineData(150, 100)]
    [InlineData(50, 50)]
    public void PageSize_Coerce(int input, int expected)
    {
        new PaginationParams { PageSize = input }.PageSize.Should().Be(expected);
    }

    [Fact]
    public void Skip_CalculadoCorretamente()
    {
        new PaginationParams { Page = 3, PageSize = 20 }.Skip.Should().Be(40);
    }
}
