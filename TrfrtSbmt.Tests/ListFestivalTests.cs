using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Festivals;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using Shouldly;
using System.Linq;

namespace TrfrtSbmt.Tests;

[TestClass]
public class ListFestivalTests
{
    [TestMethod]
    public async Task SmokeTest()
    {
        // arrange
        var query = new ListFestivals.Query();

        // act
        var result = await SendAsync(query);

        // assert
        result.ShouldNotBeNull();
        result.Festivals.ShouldNotBeNull();
        result.Festivals.ShouldNotBeEmpty();
    }

    [TestMethod]
    public async Task Pagination_1stPage()
    {
        // arrange
        var pageSize = 20;
        var query = new ListFestivals.Query
        {
            PageSize = pageSize
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Festivals.Count.ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
        result.PaginationKey.ShouldNotBeNullOrEmpty();
        result.PaginationKey.ShouldContain('|');
    }

    [TestMethod]
    public async Task Pagination_2ndPage()
    {
        // arrange
        var pageSize = 20;
        var tempQuery = new ListFestivals.Query { PageSize = pageSize };
        var tempResult = await SendAsync(tempQuery);
        var query = new ListFestivals.Query
        {
            PageSize = pageSize,
            PaginationKey = tempResult.PaginationKey
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Festivals.Count.ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        result.Festivals.Any(f => f.Id == query.PaginationKey.Split('|')[0]).ShouldBeFalse();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}
