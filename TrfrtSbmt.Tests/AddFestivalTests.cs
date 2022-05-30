using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Festivals;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using System.Linq;

namespace TrfrtSbmt.Tests;

[TestClass]
public class AddFestivalTests
{
    [TestCleanup]
    public async Task Cleanup()
    {
        Generator.EndsWith = string.Empty;
    }
    
    [TestMethod]
    public async Task SmokeTest()
    {
        // arrange
        Generator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var command = new AddFestival.AddFestivalCommand(Generator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));

        // act
        await SendAsync(command);

        // assert            
    }

    [TestMethod]
    public async Task UpdateExisting()
    {
        // arrange
        var query = new ListFestivals.ListFestivalsQuery
        {
            PageSize = 1
        };
        var result = await SendAsync(query);

        var command = new AddFestival.AddFestivalCommand(Generator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3), result.Festivals.Single().Id);

        // act
        await SendAsync(command);

        // assert            
    }
}
