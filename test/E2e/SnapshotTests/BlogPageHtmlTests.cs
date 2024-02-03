// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3016
namespace Test.E2e.SnapshotTests
{
    public class BlogPageHtmlTests : IClassFixture<DesktopFixture>
    {
        readonly DesktopFixture _DesktopFixture;

        public BlogPageHtmlTests(DesktopFixture playwrightFixture)
        {
            _DesktopFixture = playwrightFixture;
        }

        [Fact]
        public async Task Verify_BlogPageHtml_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            BlogPage blogPage = new BlogPage(page);
            await blogPage.NavigateAsync();
            Dictionary<string, string> headers = await blogPage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("Articles from the blog by Max Hamulyák · Kaylumah");

            await HtmlPageVerifier.Verify(blogPage);
        }
    }
}
