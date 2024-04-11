// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Test.E2e.SnapshotTests
{
    public class ArtifactPageTests : IClassFixture<DesktopFixture>
    {
        readonly DesktopFixture _DesktopFixture;

        public ArtifactPageTests(DesktopFixture desktopFixture)
        {
            _DesktopFixture = desktopFixture;
        }

        [Fact]
        public async Task Verify_AtomFeedXml_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            AtomFeedPage atomFeed = new AtomFeedPage(page);
            await atomFeed.NavigateAsync();
            page.Url.Should().EndWith(atomFeed.PagePath);

            string xml = await atomFeed.GetContent();
            VerifySettings settings = new VerifySettings();
            Regex regex = VerifierHelper.BaseUrl();
            settings.ScrubMatches(regex, "BaseUrl_");
            settings.ScrubInlineDateTimeOffsets("yyyy-MM-ddTHH:mm:sszzz");
            await Verifier.Verify(xml, settings);
        }

        [Fact]
        public async Task Verify_RobotTxt_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            RobotsPage robotsPage = new RobotsPage(page);
            await robotsPage.NavigateAsync();
            page.Url.Should().EndWith(robotsPage.PagePath);

            string txt = await robotsPage.GetContent();
            VerifySettings settings = new VerifySettings();
            Regex regex = VerifierHelper.BaseUrl();
            settings.ScrubMatches(regex, "BaseUrl_");
            await Verifier.Verify(txt, settings);
        }

        [Fact]
        public async Task Verify_SitemapXml_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            SitemapPage sitemapPage = new SitemapPage(page);
            await sitemapPage.NavigateAsync();
            page.Url.Should().EndWith(sitemapPage.PagePath);

            string xml = await sitemapPage.GetContent();
            VerifySettings settings = new VerifySettings();
            Regex regex = VerifierHelper.BaseUrl();
            settings.ScrubMatches(regex, "BaseUrl_");
            await Verifier.Verify(xml, settings);
        }
    }
}
