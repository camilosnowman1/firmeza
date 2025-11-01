using Firmeza.Web.Helpers;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Tests.Helpers;

public class PaginatedListTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCorrectPageAndTotalPages()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        var queryableItems = items.AsQueryable();
        int pageIndex = 2;
        int pageSize = 5;

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(queryableItems, pageIndex, pageSize);

        // Assert
        Assert.Equal(pageIndex, paginatedList.PageIndex);
        Assert.Equal(3, paginatedList.TotalPages); // 15 items / 5 per page = 3 pages
        Assert.Equal(5, paginatedList.Count); // Should contain 5 items on the second page
        Assert.Equal(6, paginatedList[0]); // First item on second page should be 6
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleFirstPageCorrectly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var queryableItems = items.AsQueryable();
        int pageIndex = 1;
        int pageSize = 5;

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(queryableItems, pageIndex, pageSize);

        // Assert
        Assert.True(paginatedList.HasNextPage);
        Assert.False(paginatedList.HasPreviousPage);
        Assert.Equal(2, paginatedList.TotalPages);
        Assert.Equal(5, paginatedList.Count);
        Assert.Equal(1, paginatedList[0]);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleLastPageCorrectly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var queryableItems = items.AsQueryable();
        int pageIndex = 2;
        int pageSize = 5;

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(queryableItems, pageIndex, pageSize);

        // Assert
        Assert.False(paginatedList.HasNextPage);
        Assert.True(paginatedList.HasPreviousPage);
        Assert.Equal(2, paginatedList.TotalPages);
        Assert.Equal(5, paginatedList.Count);
        Assert.Equal(6, paginatedList[0]);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleSinglePageCorrectly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var queryableItems = items.AsQueryable();
        int pageIndex = 1;
        int pageSize = 5;

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(queryableItems, pageIndex, pageSize);

        // Assert
        Assert.False(paginatedList.HasNextPage);
        Assert.False(paginatedList.HasPreviousPage);
        Assert.Equal(1, paginatedList.TotalPages);
        Assert.Equal(3, paginatedList.Count);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleEmptyListCorrectly()
    {
        // Arrange
        var items = new List<int>();
        var queryableItems = items.AsQueryable();
        int pageIndex = 1;
        int pageSize = 5;

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(queryableItems, pageIndex, pageSize);

        // Assert
        Assert.False(paginatedList.HasNextPage);
        Assert.False(paginatedList.HasPreviousPage);
        Assert.Equal(0, paginatedList.TotalPages);
        Assert.Equal(0, paginatedList.Count);
    }
}