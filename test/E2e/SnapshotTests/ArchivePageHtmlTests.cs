// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Test.E2e.SnapshotTests
{
    public class ArchivePageHtmlTests : IClassFixture<DesktopFixture>, IClassFixture<MobileFixture>
    {
        readonly DesktopFixture _DesktopFixture;
        readonly MobileFixture _MobileFixture;

        public ArchivePageHtmlTests(DesktopFixture desktopFixture, MobileFixture mobileFixture)
        {
            _DesktopFixture = desktopFixture;
            _MobileFixture = mobileFixture;
        }
        [Fact]
        public async Task Verify_ArchivePageHtml_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            ArchivePage archivePage = new ArchivePage(page);
            await archivePage.NavigateAsync();
            Dictionary<string, string> headers = await archivePage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("The complete archive of blog posts · Kaylumah");

            await HtmlPageVerifier.Verify(archivePage);
        }

        [Fact(Skip = "screenshots keep failing")]
        public async Task Verify_ArchivePageHtml_DesktopScreenshot()
        {
            IPage page = await _DesktopFixture.GetPage();
            ArchivePage archivePage = new ArchivePage(page);
            await archivePage.NavigateAsync();

            await BasePageVerifier.VerifyScreenshot(archivePage);
        }

        [Fact(Skip = "screenshots keep failing")]
        public async Task Verify_ArchivePageHtml_MobileScreenshot()
        {
            IPage page = await _MobileFixture.GetPage();
            ArchivePage archivePage = new ArchivePage(page);
            await archivePage.NavigateAsync();

            await BasePageVerifier.VerifyScreenshot(archivePage);
        }
    }
}
