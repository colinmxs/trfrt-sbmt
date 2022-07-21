using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Forts;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using System.Linq;
using TrfrtSbmt.Api.Features.Festivals;
using CodenameGenerator;
using TrfrtSbmt.Api.Features.Labels;

namespace TrfrtSbmt.Tests.Forts;

[TestClass]
public class DeleteFortTests
{
    private FestivalViewModel fest;
    private FortViewModel fort;

    [TestInitialize]
    public async Task Initialize()
    {
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var addFest = new AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
        fest = await SendAsync(addFest);
        
        NameGenerator.SetParts(WordBank.Nouns);
        NameGenerator.EndsWith = $"Fort";
        var command = new AddFort.AddFortCommand(NameGenerator.Generate(), Lorem) 
        {
            FestivalId = fest.Id
        };
        fort = await SendAsync(command);
    }
    
    [TestCleanup]
    public async Task Cleanup()
    {
        var deleteFest1 = new DeleteFestivalCommand(fest.Id);
        await SendAsync(deleteFest1);
        
        NameGenerator.SetParts(WordBank.Verbs, WordBank.Nouns);
        NameGenerator.EndsWith = string.Empty;
    }

    [TestMethod]
    public async Task SmokeTest()
    {
        // arrange
        var command = new DeleteFort.DeleteFortCommand(fort.Id);

        // act
        await SendAsync(command);

        // assert            
    }
}
