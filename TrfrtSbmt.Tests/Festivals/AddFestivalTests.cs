using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Festivals;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using System.Linq;
using Shouldly;

namespace TrfrtSbmt.Tests.Festivals;

[TestClass]
public class AddFestivalTests
{
    FestivalViewModel? tempFest1;
    FestivalViewModel? tempFest2;

    [TestInitialize]
    public async Task Initialize()
    {
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var addFest = new AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
        tempFest2 = await SendAsync(addFest);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        NameGenerator.EndsWith = string.Empty;
        if (tempFest1 != null) 
        {
            var deleteFest1 = new DeleteFestivalCommand(tempFest1.Id);
            await SendAsync(deleteFest1);
        }
        if(tempFest2 != null)
        {
            var deleteFest2 = new DeleteFestivalCommand(tempFest2.Id);
            await SendAsync(deleteFest2);
        }
    }

    [TestMethod]    
    public async Task SmokeTest()
    {
        // arrange
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var command = new AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));

        // act
        tempFest1 = await SendAsync(command);

        // assert
        tempFest1.Id.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task UpdateExisting()
    {
        // arrange
        var command = new AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3), tempFest2.Id);

        // act
        await SendAsync(command);

        // assert            
    }
}
