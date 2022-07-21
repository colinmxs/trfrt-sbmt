using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrfrtSbmt.Api.Features.Festivals;
using System;
using static TrfrtSbmt.Tests.TestFixture;
using System.Threading.Tasks;
using Shouldly;
using System.Linq;
using System.Collections.Generic;
using static TrfrtSbmt.Api.Features.Submissions.ListSubmissions;
using TrfrtSbmt.Api.Features.Submissions;
using TrfrtSbmt.Api.Features.Forts;
using CodenameGenerator;

namespace TrfrtSbmt.Tests.Submissions;

[TestClass]
public class ListSubmissionTests
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
        for (int i = 0; i < 100; i++)
        {
            var addSub = new AddSubmission.AddSubmissionCommand(GeneratorMethods.GenerateName(), i % 3 == 0 ? "Idaho" : GeneratorMethods.GenerateState(), i % 3 == 0 ? "Boise" : GeneratorMethods.GenerateCity(), i % 10 == 0 ? "United States" : GeneratorMethods.GenerateCountry(), Lorem, GeneratorMethods.GeneratePictureUrl(), "https://www.reddit.com/r/U2Band/", GeneratorMethods.GenerateGenre().Distinct(), Lorem, new SocialLinksVm("https://open.spotify.com/artist/51Blml2LZPmy7TTiAg47vQ", "https://music.apple.com/us/artist/u2/78500", "https://suckling.bandcamp.com/releases", "https://soundcloud.com/u2", new string[3] { "https://www.youtube.com/watch?v=ujNeHIo7oTE&ab_channel=U2VEVO", "https://www.youtube.com/watch?v=98W9QuMq-2k&ab_channel=U2VEVO", "https://www.youtube.com/watch?v=co6WMzDOh1o&ab_channel=U2VEVO" }, "https://www.facebook.com/u2", "https://twitter.com/u2", "https://www.instagram.com/u2", "https://www.tiktok.com/@u2?lang=en"), GeneratorMethods.GenerateContactInfo())
            {
                FestivalId = festival.Id,
                FortId = fort.Id
            };
            submissions.Add(await SendAsync(addSub));
        };
        await Task.WhenAll(tasks);
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
        var query = new ListSubmissions.ListSubmissionsQuery(festival.Id, fort.Id);

        // act
        var result = await SendAsync(query);

        // assert
        result.ShouldNotBeNull();
        result.Submissions.ShouldNotBeNull();
        result.Submissions.ShouldNotBeEmpty();
    }

    [TestMethod]
    public async Task Pagination_1stPage()
    {
        // arrange
        var pageSize = 20;
        var query = new ListSubmissionsQuery(festival.Id, fort.Id)
        {
            PageSize = pageSize
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Submissions.Count().ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
        result.PaginationKey.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task Pagination_2ndPage()
    {
        // arrange
        var pageSize = 20;
        var tempQuery = new ListSubmissions.ListSubmissionsQuery(festival.Id, fort.Id) { PageSize = pageSize };
        var tempResult = await SendAsync(tempQuery);
        var query = new ListSubmissions.ListSubmissionsQuery(festival.Id, fort.Id)
        {
            PageSize = pageSize,
            PaginationKey = tempResult.PaginationKey
        };

        // act
        var result = await SendAsync(query);

        // assert
        result.Submissions.Count().ShouldBe(pageSize);
        result.PageSize.ShouldBe(pageSize);
    }
}
