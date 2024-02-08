// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Reqnroll;
using Test.Unit.Extensions;
using Test.Unit.Utilities;
using VerifyXunit;

namespace Test.Unit.Component.Manager.Site.Steps
{
    [Binding]
    public class VerifyArtifactAccessMockSteps
    {
        readonly ArtifactAccessMock _ArtifactAccess;
        readonly ScenarioContext _ScensarioContext;

        public VerifyArtifactAccessMockSteps(ArtifactAccessMock artifactAccess, ScenarioContext scenarioContext)
        {
            _ArtifactAccess = artifactAccess;
            _ScensarioContext = scenarioContext;
        }

        [Then("the atom feed '(.*)' is verified:")]
        public async Task ThenTheAtomFeedIsVerified(string feedPath)
        {
            string feed = _ArtifactAccess.GetString(feedPath);
            await Verifier.Verify(feed)
                .UseMethodName(_ScensarioContext.ToVerifyMethodName("AtomFeed"));
        }

        [Then("the sitemap '(.*)' is verified:")]
        public async Task ThenTheSitemapIsVerified(string sitemapPath)
        {
            string sitemap = _ArtifactAccess.GetString(sitemapPath);
            await Verifier.Verify(sitemap)
                .UseMethodName(_ScensarioContext.ToVerifyMethodName("Sitemap"));
        }

        [Then("the html '(.*)' is verified:")]
        public async Task ThenTheHtmlIsVerified(string htmlPath)
        {
            string htmlPage = _ArtifactAccess.GetString(htmlPath);
            await Verifier.Verify(htmlPage)
                .UseMethodName(_ScensarioContext.ToVerifyMethodName("Html"));
        }
    }
}
