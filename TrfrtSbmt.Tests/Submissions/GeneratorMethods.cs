using TrfrtSbmt.Api.Features.Submissions;
using static TrfrtSbmt.Tests.TestFixture;
using CodenameGenerator;
using System.Collections.Generic;

namespace TrfrtSbmt.Tests.Submissions;

public static class GeneratorMethods
{
    public static IEnumerable<string> GenerateGenre()
    {
        var genres = new string[] { "Rock", "Country", "Rap", "Metal", "Indie", "Folk", "Pop", "Jazz", "Hip-Hop", "Electronic", "Blues", "Classical", "Reggae", "Soul", "R&B", "Dance", "Alternative", "Disco", "House", "Techno", "Other" };

        // return 3 random genres
        for (int i = 0; i < 3; i++)
        {
            yield return genres[Rand.Next(0, genres.Length)];
        }
    }

    public static string GenerateCity()
    {
        NameGenerator.SetParts(WordBank.Cities);
        return NameGenerator.Generate();
    }

    public static string GenerateState()
    {
        NameGenerator.SetParts(WordBank.StateNames);
        return NameGenerator.Generate();
    }

    public static string GenerateName()
    {
        NameGenerator.SetParts(WordBank.Verbs, WordBank.Nouns);
        NameGenerator.EndsWith = string.Empty;
        return NameGenerator.Generate();
    }
    public static ContactInfoVm GenerateContactInfo()
    {
        NameGenerator.SetParts(WordBank.FirstNames, WordBank.LastNames);
        var name = NameGenerator.Generate();
        var email = $"{name.Replace(" ", "")}@gfail.com";
        return new ContactInfoVm(name, $"{Rand.Next(100, 999)}-{Rand.Next(100, 999)}-{Rand.Next(1000, 9999)}", email, "Friend", null, null, null, null);
    }

    public static string GenerateCountry()
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

    public static string GeneratePictureUrl()
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
}
    
