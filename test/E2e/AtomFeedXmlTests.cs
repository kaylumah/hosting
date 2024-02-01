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
    public class AtomFeedXmlTests : IClassFixture<PlaywrightFixture>
    {
        readonly PlaywrightFixture _PlaywrightFixture;

        public AtomFeedXmlTests(PlaywrightFixture playwrightFixture)
        {
            _PlaywrightFixture = playwrightFixture;
        }

        [Fact]
        public async Task Verify_AtomFeedXml_Contents()
        {
            IPage page = await _PlaywrightFixture.GetPage();
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
    }
}
