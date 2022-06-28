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
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Tests.Submissions;

[TestClass]
public class GetUploadUrlTests
{
    [TestMethod]
    public async Task SmokeTest()
    {
        // arrange
        var query = new GetUploadUrl.Query("test.jpg", "Testiest Test", "Desc", "image/jpeg");

        // act
        var url = await SendAsync(query);

        // assert
        url.ShouldNotBeNull();
        url.UploadUrl.ShouldNotBeNullOrEmpty();
    }
}
