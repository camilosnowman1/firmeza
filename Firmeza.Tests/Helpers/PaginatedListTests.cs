using NUnit.Framework;
using Firmeza.Web.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Needed for AsQueryable and async EF methods

namespace Firmeza.Tests.Helpers;

[TestFixture]
public class PaginatedListTests
{
    private List<int> _sourceList;

    [SetUp]
    public void Setup()
    {
        _sourceList = Enumerable.Range(1, 100).ToList();
    }

    [Test]
    public async Task CreateAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 3; // Requesting the 3rd page

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(_sourceList.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.IsNotNull(paginatedList);
        Assert.AreEqual(pageIndex, paginatedList.PageIndex);
        Assert.AreEqual(10, paginatedList.Count); // Should contain 10 items
        Assert.AreEqual(21, paginatedList[0]); // First item on 3rd page (0-indexed skip)
        Assert.AreEqual(10, paginatedList.TotalPages);
    }

    [Test]
    public async Task CreateAsync_ShouldHandleFirstPage()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 1;

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(_sourceList.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.IsNotNull(paginatedList);
        Assert.AreEqual(pageIndex, paginatedList.PageIndex);
        Assert.AreEqual(10, paginatedList.Count);
        Assert.AreEqual(1, paginatedList[0]);
        Assert.IsTrue(paginatedList.HasNextPage);
        Assert.IsFalse(paginatedList.HasPreviousPage);
    }

    [Test]
    public async Task CreateAsync_ShouldHandleLastPage()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 10; // Last page

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(_sourceList.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.IsNotNull(paginatedList);
        Assert.AreEqual(pageIndex, paginatedList.PageIndex);
        Assert.AreEqual(10, paginatedList.Count);
        Assert.AreEqual(91, paginatedList[0]); // First item on last page
        Assert.IsFalse(paginatedList.HasNextPage);
        Assert.IsTrue(paginatedList.HasPreviousPage);
    }

    [Test]
    public async Task CreateAsync_ShouldHandleEmptySource()
    {
        // Arrange
        var emptyList = new List<int>().AsQueryable();
        var pageSize = 10;
        var pageIndex = 1;

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(emptyList, pageIndex, pageSize);

        // Assert
        Assert.IsNotNull(paginatedList);
        Assert.AreEqual(0, paginatedList.Count);
        Assert.AreEqual(0, paginatedList.TotalPages);
        Assert.IsFalse(paginatedList.HasNextPage);
        Assert.IsFalse(paginatedList.HasPreviousPage);
    }

    [Test]
    public async Task CreateAsync_ShouldHandlePageOutOfRange()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 15; // Page beyond total pages

        // Act
        var paginatedList = await PaginatedList<int>.CreateAsync(_sourceList.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.IsNotNull(paginatedList);
        Assert.AreEqual(10, paginatedList.TotalPages); // Still 10 total pages
        Assert.AreEqual(0, paginatedList.Count); // No items on this page
    }
}
