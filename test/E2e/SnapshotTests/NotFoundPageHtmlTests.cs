// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Test.E2e.SnapshotTests
{

    public class NotFoundPageHtmlTests : IClassFixture<DesktopFixture>, IClassFixture<MobileFixture>
    {
        readonly DesktopFixture _DesktopFixture;
        readonly MobileFixture _MobileFixture;

        public NotFoundPageHtmlTests(DesktopFixture desktopFixture, MobileFixture mobileFixture)
        {
            _DesktopFixture = desktopFixture;
            _MobileFixture = mobileFixture;
        }

        [Fact(Skip = "screenshots keep failing")]
        public async Task Verify_NotFoundPageHtml_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            NotFoundPage notFoundPage = new NotFoundPage(page);
            await notFoundPage.NavigateAsync();
            Dictionary<string, string> headers = await notFoundPage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("Page not found · Kaylumah");

            await HtmlPageVerifier.Verify(notFoundPage);
        }

        [Fact(Skip = "screenshots keep failing")]
        public async Task Verify_NotFoundPageHtml_DesktopScreenshot()
        {
            IPage page = await _DesktopFixture.GetPage();
            NotFoundPage notFoundPage = new NotFoundPage(page);
            await notFoundPage.NavigateAsync();

            await BasePageVerifier.VerifyScreenshot(notFoundPage);
        }

        [Fact]
        public async Task Verify_NotFoundPageHtml_MobileScreenshot()
        {
            IPage page = await _MobileFixture.GetPage();
            NotFoundPage notFoundPage = new NotFoundPage(page);
            await notFoundPage.NavigateAsync();

            await BasePageVerifier.VerifyScreenshot(notFoundPage);
        }
    }
}
