// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3016
namespace Test.E2e
{
    public class HomePageHtmlTests : IClassFixture<PlaywrightFixture>
    {
        readonly PlaywrightFixture _PlaywrightFixture;

        public HomePageHtmlTests(PlaywrightFixture playwrightFixture)
        {
            _PlaywrightFixture = playwrightFixture;
        }

        [Fact]
        public async Task Verify_HomePageHtml_Contents()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            HomePage homePage = new HomePage(page);
            await homePage.NavigateAsync();
            string title = await page.TitleAsync();
            title.Should().Be("Max Hamulyák · Kaylumah");

            await HtmlPageVerifier.Verify(homePage);
        }

        [Fact]
        public async Task Verify_HomePageHtml_Screenshot()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            HomePage homePage = new HomePage(page);
            await homePage.NavigateAsync();

            await BasePageVerifier.VerifyScreenshot(homePage);
        }
    }
}
