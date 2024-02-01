// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using VerifyTests;
using VerifyXunit;
using Xunit;

#pragma warning disable CS3016
namespace Test.E2e
{
    public class SitemapXmlTests : IClassFixture<PlaywrightFixture>
    {
        readonly PlaywrightFixture _PlaywrightFixture;

        public SitemapXmlTests(PlaywrightFixture playwrightFixture)
        {
            _PlaywrightFixture = playwrightFixture;
        }

        [Fact]
        public async Task Verify_SitemapXml_Contents()
        {
            IPage page = await _PlaywrightFixture.GetPage();
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
