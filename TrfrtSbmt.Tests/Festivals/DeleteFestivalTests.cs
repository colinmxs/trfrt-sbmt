using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Festivals;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using System.Linq;

namespace TrfrtSbmt.Tests.Festivals;

[TestClass]
public class DeleteFestivalTests
{
    FestivalViewModel festival;

    [TestInitialize]
    public async Task Initialize()
    {
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var addFestivalCommand = new AddFestival.AddFestivalCommand(NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
        festival = await SendAsync(addFestivalCommand);
    }


    [TestCleanup]
    public async Task Cleanup()
    {
        NameGenerator.EndsWith = string.Empty;
    }

    [TestMethod]
    public async Task SmokeTest()
    {
        // arrange
        var command = new DeleteFestival.DeleteFestivalCommand(festival.Id);

        // act
        await SendAsync(command);

        // assert            
    }
}
