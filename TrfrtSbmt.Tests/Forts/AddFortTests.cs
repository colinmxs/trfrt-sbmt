using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Forts;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using System.Linq;
using TrfrtSbmt.Api.DataModels;
using TrfrtSbmt.Api.Features.Festivals;
using CodenameGenerator;

namespace TrfrtSbmt.Tests.Forts;

[TestClass]
public class AddFortTests
{
    FestivalViewModel festival;

    [TestInitialize]
    public async Task Initialize()
    {
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var addFestivalCommand = new AddFestival.AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
        festival = await SendAsync(addFestivalCommand);
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
        NameGenerator.SetParts(WordBank.Nouns);
        NameGenerator.EndsWith = $"Fort";
        var command = new AddFort.AddFortCommand(NameGenerator.Generate()) 
        {
            FestivalId = festival.Id
        };

        // act
        await SendAsync(command);

        // assert            
    }
}
