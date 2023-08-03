// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Test.Specflow.Extensions;
using Test.Specflow.Utilities;
using VerifyXunit;

namespace Test.Specflow.Component.Manager.Site.Steps;

[Binding]
public class VerifyArtifactAccessMockSteps
{
    private readonly ArtifactAccessMock _artifactAccess;
    private readonly ScenarioContext _scenarioContext;

    public VerifyArtifactAccessMockSteps(ArtifactAccessMock artifactAccess, ScenarioContext scenarioContext)
    {
        _artifactAccess = artifactAccess;
        _scenarioContext = scenarioContext;
    }

    [Then("the atom feed '(.*)' is verified:")]
    public async Task ThenTheAtomFeedIsVerified(string feedPath)
    {
        var feed = _artifactAccess.GetString(feedPath);
        await Verifier.Verify(feed)
            .UseMethodName(_scenarioContext.ToVerifyMethodName("AtomFeed"));
    }

    [Then("the sitemap '(.*)' is verified:")]
    public async Task ThenTheSitemapIsVerified(string sitemapPath)
    {
        var sitemap = _artifactAccess.GetString(sitemapPath);
        await Verifier.Verify(sitemap)
            .UseMethodName(_scenarioContext.ToVerifyMethodName("Sitemap"));
    }

    [Then("the html '(.*)' is verified:")]
    public async Task ThenTheHtmlIsVerified(string htmlPath)
    {
        var htmlPage = _artifactAccess.GetString(htmlPath);
        await Verifier.Verify(htmlPage)
            .UseMethodName(_scenarioContext.ToVerifyMethodName("Html"));
    }
}
