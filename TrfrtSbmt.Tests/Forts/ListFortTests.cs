using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Forts;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using Shouldly;
using System.Linq;
using TrfrtSbmt.Api.Features.Festivals;
using CodenameGenerator;

namespace TrfrtSbmt.Tests.Forts;

[TestClass]
public class ListFortTests
{
    FestivalViewModel festival;

    [TestInitialize]
    public async Task Initialize()
    {
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var addFestivalCommand = new AddFestival.AddFestivalCommand(NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
        festival = await SendAsync(addFestivalCommand);

        NameGenerator.SetParts(WordBank.Nouns);
        NameGenerator.EndsWith = $"Fort";
        for (int i = 0; i < 40; i++)
        {
            var command = new AddFort.AddFortCommand(NameGenerator.Generate()) 
            {
                FestivalId = festival.Id
            };
            await SendAsync(command);
        }
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        var deleteFest1 = new DeleteFestival.DeleteFestivalCommand(festival.Id);
        await SendAsync(deleteFest1);

        NameGenerator.SetParts(WordBank.Verbs, WordBank.Nouns);
        NameGenerator.EndsWith = string.Empty;
    }

    [TestMethod]
    public async Task SmokeTest()
    {
        // arrange
        var query = new ListForts.ListFortsQuery(festival.Id);

        // act
        var result = await SendAsync(query);

        // assert
        result.ShouldNotBeNull();
        result.Forts.ShouldNotBeNull();
        result.Forts.ShouldNotBeEmpty();
    }

    [TestMethod]
    public async Task Pagination_1stPage()
    {
        // arrange
        var pageSize = 20;
        var query = new ListForts.ListFortsQuery(festival.Id)
        {
            PageSize = pageSize
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Forts.Count().ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
        result.PaginationKey.ShouldNotBeNullOrEmpty();
        result.PaginationKey.ShouldContain('|');
    }

    [TestMethod]
    public async Task Pagination_2ndPage()
    {
        // arrange
        var pageSize = 20;
        var tempQuery = new ListForts.ListFortsQuery(festival.Id) { PageSize = pageSize };
        var tempResult = await SendAsync(tempQuery);
        var query = new ListForts.ListFortsQuery(festival.Id)
        {
            PageSize = pageSize,
            PaginationKey = tempResult.PaginationKey
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Forts.Count().ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        result.Forts.Any(f => f.Id == query.PaginationKey.Split('|')[0]).ShouldBeFalse();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}
