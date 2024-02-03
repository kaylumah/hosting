// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Test.E2e.SnapshotTests
{
    public class AboutPageHtmlTests : IClassFixture<DesktopFixture>
    {
        readonly DesktopFixture _DesktopFixture;

        public AboutPageHtmlTests(DesktopFixture playwrightFixture)
        {
            _DesktopFixture = playwrightFixture;
        }

        [Fact]
        public async Task Verify_AboutPageHtml_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            AboutPage aboutPage = new AboutPage(page);
            await aboutPage.NavigateAsync();

            Dictionary<string, string> headers = await aboutPage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("All about Max Hamulyák from personal to Curriculum Vitae · Kaylumah");

            await HtmlPageVerifier.Verify(aboutPage);
        }
    }
}
