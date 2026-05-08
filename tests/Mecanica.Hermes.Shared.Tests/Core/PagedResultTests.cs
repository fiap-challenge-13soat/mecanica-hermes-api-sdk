using Mecanica.Hermes.Shared.Core.Pagination;

namespace Mecanica.Hermes.Shared.Tests.Core;

public class PagedResultTests
{
    [Fact]
    public void Create_PreservaItensETotal()
    {
        var items = new[] { 1, 2, 3 };

        var paged = PagedResult<int>.Create(items, page: 1, pageSize: 10, totalCount: 3);

        paged.Items.Should().BeEquivalentTo(items);
        paged.Page.Should().Be(1);
        paged.PageSize.Should().Be(10);
        paged.TotalCount.Should().Be(3);
    }

    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(95, 10, 10)]
    public void TotalPages_ArredondaParaCima(int total, int pageSize, int expected)
    {
        var paged = PagedResult<int>.Create([], page: 1, pageSize: pageSize, totalCount: total);

        paged.TotalPages.Should().Be(expected);
    }

    [Fact]
    public void HasPreviousPage_FalseNaPrimeira_TrueNoMeio()
    {
        PagedResult<int>.Create([], page: 1, pageSize: 10, totalCount: 50).HasPreviousPage.Should().BeFalse();
        PagedResult<int>.Create([], page: 2, pageSize: 10, totalCount: 50).HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_TrueNoMeio_FalseNaUltima()
    {
        PagedResult<int>.Create([], page: 1, pageSize: 10, totalCount: 25).HasNextPage.Should().BeTrue();
        PagedResult<int>.Create([], page: 3, pageSize: 10, totalCount: 25).HasNextPage.Should().BeFalse();
    }
}
