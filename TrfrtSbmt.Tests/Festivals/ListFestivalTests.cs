﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Festivals;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using Shouldly;
using System.Linq;
using System.Collections.Generic;

namespace TrfrtSbmt.Tests.Festivals;

[TestClass]
public class ListFestivalTests
{
    List<FestivalViewModel> festivals = new List<FestivalViewModel>();
    [TestInitialize]
    public async Task Init()
    {
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        for (int i = 0; i < 100; i++)
        {
            var addFest = new AddFestival.AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
            festivals.Add(await SendAsync(addFest));
        }
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        foreach (var fest in festivals)
        {
            var deleteCommand = new DeleteFestival.DeleteFestivalCommand(fest.Id);
            await SendAsync(deleteCommand);
        }
    }
    [TestMethod]
    public async Task SmokeTest()
    {
        // arrange
        var query = new ListFestivals.ListFestivalsQuery();

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
        var query = new ListFestivals.ListFestivalsQuery
        {
            PageSize = pageSize
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Festivals.Count().ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
        result.PaginationKey.ShouldNotBeNullOrEmpty();
        result.PaginationKey.ShouldContain('|');
    }

    [TestMethod]
    public async Task Pagination_2ndPage()
    {
        // arrange
        var pageSize = 20;
        var tempQuery = new ListFestivals.ListFestivalsQuery { PageSize = pageSize };
        var tempResult = await SendAsync(tempQuery);
        var query = new ListFestivals.ListFestivalsQuery
        {
            PageSize = pageSize,
            PaginationKey = tempResult.PaginationKey
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Festivals.Count().ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        result.Festivals.Any(f => f.Id == query.PaginationKey.Split('|')[0]).ShouldBeFalse();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}
