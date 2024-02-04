// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Test.E2e.SnapshotTests
{
    public class HomePageHtmlTests : IClassFixture<DesktopFixture>, IClassFixture<MobileFixture>
    {
        readonly DesktopFixture _DesktopFixture;
        readonly MobileFixture _MobileFixture;

        public HomePageHtmlTests(DesktopFixture desktopFixture, MobileFixture mobileFixture)
        {
            _DesktopFixture = desktopFixture;
            _MobileFixture = mobileFixture;
        }

        [Fact]
        public async Task Verify_HomePageHtml_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            HomePage homePage = new HomePage(page);
            await homePage.NavigateAsync();
            string title = await page.TitleAsync();
            title.Should().Be("Max Hamulyák · Kaylumah");

            await HtmlPageVerifier.Verify(homePage);
        }

        [Fact(Skip = "Screenshot compare not ready")]
        public async Task Verify_HomePageHtml_DesktopScreenshot()
        {
            IPage page = await _DesktopFixture.GetPage();
            HomePage homePage = new HomePage(page);
            await homePage.NavigateAsync();

            await BasePageVerifier.VerifyScreenshot(homePage);
        }

        [Fact(Skip = "Screenshot compare not ready")]
        public async Task Verify_HomePageHtml_MobileScreenshot()
        {
            IPage page = await _MobileFixture.GetPage();
            HomePage homePage = new HomePage(page);
            await homePage.NavigateAsync();

            await BasePageVerifier.VerifyScreenshot(homePage);
        }
    }
}
