using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Submissions;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using System.Linq;
using TrfrtSbmt.Api.DataModels;
using TrfrtSbmt.Api.Features.Festivals;
using CodenameGenerator;
using TrfrtSbmt.Api.Features.Forts;

namespace TrfrtSbmt.Tests.Submissions;

[TestClass]
public class AddSubmissionTests
{
    FestivalViewModel festival;
    FortViewModel fort;

    [TestInitialize]
    public async Task Initialize()
    {
        NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
        var addFestivalCommand = new AddFestival.AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
        festival = await SendAsync(addFestivalCommand);

        NameGenerator.SetParts(WordBank.Nouns);
        NameGenerator.EndsWith = $"Fort";
        var command = new AddFort.AddFortCommand(NameGenerator.Generate())
        {
            FestivalId = festival.Id
        };
        fort = await SendAsync(command);
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
        var command = new AddSubmission.AddSubmissionCommand(GenerateName(), GenerateState(), GenerateCity(), GenerateCountry(), Lorem, GeneratePictureUrl(), "https://reddit.com/", GenerateGenre(), new AddSubmission.SocialLinksVm(null, null, null, null, null, null, null, null, null), new AddSubmission.ContactInfoVm(null, null, null, null, null, null, null, null))
        {
            FestivalId = festival.Id,
            FortId = fort.Id
        };

        // act
        await SendAsync(command);

        // assert
        //submission.Id.IsNotNull();
    }

    private string GenerateCountry()
    {
        // generate a random number either 1 or 0
        var randomNumber = Rand.Next(0, 2);
        if (randomNumber == 0)
        {
            return "United States";
        }
        else
        {
            NameGenerator.SetParts(WordBank.Countries);
            return NameGenerator.Generate();
        }
    }

    private string GeneratePictureUrl()
    {
        var urls = new string[] {
            "https://static.boredpanda.com/blog/wp-content/uploads/2019/02/funny-awkward-metal-bands-photos-fb1.png",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQBbScoM0mFImRYw4S1SxtNCsWD-fcNctp46vawo_HOFtsQjPSN8xMcc9dfBHeigzuaydk&usqp=CAU",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQVjP2J0-0lu1-1iFwU4-cldG9r6MkYyYJjYtWTxp-8cd-pitJFKsFZzdwIQYTS-wScOkk&usqp=CAU",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR_i7yvDPjqpmwYggNRZZ4lEijavyGyw-FTpPci0O7YYqWakaRDrmyJ9WVIrsK8oyddhJI&usqp=CAU",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcScmq03faAQ-uqXTYkLTr6FVUDs3quwdGK1Qq9np-n76QxsnSEW1HTQynQ9uC8HR-RnT4g&usqp=CAU",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ0bTlUeZTDXoCtcayb6-pMUWTCnZVIysmygRFUWOMz-jsxjTapmDTKLfoyeZmBlDdhlNE&usqp=CAU"
        };
        return urls[Rand.Next(0, urls.Length)];
    }

    private string GenerateGenre()
    {
        var genres = new string[] { "Rock", "Country", "Rap", "Metal", "Indie", "Folk", "Pop", "Jazz", "Hip-Hop", "Electronic", "Blues", "Classical", "Reggae", "Soul", "R&B", "Dance", "Alternative", "Disco", "House", "Techno", "Other" };
        return genres[Rand.Next(0, genres.Length)];
    }

    private string GenerateCity()
    {
        NameGenerator.SetParts(WordBank.Cities);
        return NameGenerator.Generate();
    }

    private string GenerateState()
    {
        NameGenerator.SetParts(WordBank.StateNames);
        return NameGenerator.Generate();
    }

    private string GenerateName()
    {
        NameGenerator.SetParts(WordBank.Verbs, WordBank.Nouns);
        NameGenerator.EndsWith = string.Empty;
        return NameGenerator.Generate();
    }
}
