using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Forts;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using System.Linq;
using TrfrtSbmt.Api.DataModels;
using TrfrtSbmt.Api.Features.Festivals;
using CodenameGenerator;
using TrfrtSbmt.Api.Features.Labels;
using TrfrtSbmt.Api.Features.Submissions;
using System.Collections.Generic;
using TrfrtSbmt.Tests.Submissions;
using Shouldly;

namespace TrfrtSbmt.Tests.Labels
{
    [TestClass]
    public class ListLabelsTests
    {
        FestivalViewModel festival;
        FortViewModel fort;

        List<SubmissionViewModel> submissions = new List<SubmissionViewModel>();
        [TestInitialize]
        public async Task Init()
        {
            NameGenerator.EndsWith = $" Festival 20{Rand.Next(10, 30)}!";
            var addFestivalCommand = new AddFestivalCommand(true, NameGenerator.Generate(), Lorem, DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddMonths(3));
            festival = await SendAsync(addFestivalCommand);

            NameGenerator.SetParts(WordBank.Nouns);
            NameGenerator.EndsWith = $"fort";
            var command = new AddFort.AddFortCommand("Music", Lorem)
            {
                FestivalId = festival.Id
            };
            fort = await SendAsync(command);

            List<Task> tasks = new List<Task>();
            var addSub = new AddSubmission.AddSubmissionCommand(GeneratorMethods.GenerateName(), GeneratorMethods.GenerateState(), GeneratorMethods.GenerateCity(), GeneratorMethods.GenerateCountry(), Lorem, GeneratorMethods.GeneratePictureUrl(), "https://www.reddit.com/r/U2Band/", GeneratorMethods.GenerateGenre().Distinct(), Lorem, new SocialLinksVm("https://open.spotify.com/artist/51Blml2LZPmy7TTiAg47vQ", "https://music.apple.com/us/artist/u2/78500", "https://suckling.bandcamp.com/releases", "https://soundcloud.com/u2", new string[3] { "https://www.youtube.com/watch?v=ujNeHIo7oTE&ab_channel=U2VEVO", "https://www.youtube.com/watch?v=98W9QuMq-2k&ab_channel=U2VEVO", "https://www.youtube.com/watch?v=co6WMzDOh1o&ab_channel=U2VEVO" }, "https://www.facebook.com/u2", "https://twitter.com/u2", "https://www.instagram.com/u2", "https://www.tiktok.com/@u2?lang=en"), GeneratorMethods.GenerateContactInfo())
            {
                FestivalId = festival.Id,
                FortId = fort.Id
            };
            submissions.Add(await SendAsync(addSub));

            for (int i = 0; i < 40; i++)
            {
                var labelCommand = new AddLabel.AddLabelCommand(i.ToString(), new string[] { submissions.Single().Id }) 
                {
                    FestivalId = festival.Id,
                };
                await SendAsync(labelCommand);
            }
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            var deleteCommand = new DeleteFestivalCommand(festival.Id);
            await SendAsync(deleteCommand);
        }

        [TestMethod]
        public async Task SmokeTest()
        {
            // arrange
            var query = new ListLabels.ListLabelsQuery(festival.Id);

            // act
            var results = await SendAsync(query);

            // assert
            results.ShouldNotBeNull();
            results.Labels.Count().ShouldBe(20);
            results.PaginationKey.ShouldNotBeNullOrEmpty();
        }
    }
}
