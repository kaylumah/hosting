// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3016
namespace Test.E2e.SnapshotTests
{

    public class NotFoundPageHtmlTests : IClassFixture<DesktopFixture>
    {
        readonly DesktopFixture _DesktopFixture;

        public NotFoundPageHtmlTests(DesktopFixture playwrightFixture)
        {
            _DesktopFixture = playwrightFixture;
        }

        [Fact]
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
    }
}
