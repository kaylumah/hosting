// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3016
namespace Test.E2e
{
    public class ArchivePageHtmlTests : IClassFixture<DesktopFixture>
    {
        readonly DesktopFixture _DesktopFixture;

        public ArchivePageHtmlTests(DesktopFixture playwrightFixture)
        {
            _DesktopFixture = playwrightFixture;
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
    }
}
