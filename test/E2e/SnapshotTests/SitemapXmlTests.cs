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
    public class SitemapXmlTests : IClassFixture<DesktopFixture>
    {
        readonly DesktopFixture _DesktopFixture;

        public SitemapXmlTests(DesktopFixture desktopFixture)
        {
            _DesktopFixture = desktopFixture;
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
