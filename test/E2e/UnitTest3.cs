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
namespace Test.E2e
{
    public class UnitTest3 : IClassFixture<PlaywrightFixture>
    {
        readonly PlaywrightFixture _PlaywrightFixture;

        public UnitTest3(PlaywrightFixture playwrightFixture)
        {
            _PlaywrightFixture = playwrightFixture;
        }

        [Fact]
        public async Task Test_AtomFeed()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            AtomFeedPage atomFeed = new AtomFeedPage(page);
            await atomFeed.NavigateAsync();
            page.Url.Should().EndWith(atomFeed.PagePath);
            Dictionary<string, string> headers = await atomFeed.GetHeaders();

            byte[] bytes = await atomFeed.PageResponse.BodyAsync();
            SyndicationFeed feed = bytes.ToSyndicationFeed();
            feed.Title.Text.Should().Be("Max Hamulyák · Kaylumah");
        }

        [Fact]
        public async Task Test_Sitemap()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            SitemapPage sitemapPage = new SitemapPage(page);
            await sitemapPage.NavigateAsync();

            page.Url.Should().EndWith(sitemapPage.PagePath);

            Dictionary<string, string> headers = await sitemapPage.GetHeaders();

            byte[] bytes = await sitemapPage.PageResponse.BodyAsync();
            SiteMap sitemap = bytes.ToSiteMap();
            string url = _PlaywrightFixture.GetBaseUrl();
            sitemap.Items.ToList().ElementAt(0).Url.Should().Be(url);
        }

        [Fact]
        public async Task Test_Robots()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            RobotsPage robotsPage = new RobotsPage(page);
            await robotsPage.NavigateAsync();

            page.Url.Should().EndWith(robotsPage.PagePath);

            Dictionary<string, string> headers = await robotsPage.GetHeaders();

            byte[] bytes = await robotsPage.PageResponse.BodyAsync();
            UTF8Encoding encoding = new UTF8Encoding(false);
            string robots = encoding.GetString(bytes);
        }

        [Fact]
        public async Task Test_HomePage()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            HomePage homePage = new HomePage(page);
            await homePage.NavigateAsync();
            Dictionary<string, string> headers = await homePage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("Max Hamulyák · Kaylumah");

            await HtmlPageVerifier.Verify(homePage);
        }

        [Fact]
        public async Task Test_AboutPage()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            AboutPage aboutPage = new AboutPage(page);
            await aboutPage.NavigateAsync();

            Dictionary<string, string> headers = await aboutPage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("All about Max Hamulyák from personal to Curriculum Vitae · Kaylumah");

            await HtmlPageVerifier.Verify(aboutPage);
        }

        [Fact]
        public async Task Test_BlogPage()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            BlogPage blogPage = new BlogPage(page);
            await blogPage.NavigateAsync();
            Dictionary<string, string> headers = await blogPage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("Articles from the blog by Max Hamulyák · Kaylumah");

            await HtmlPageVerifier.Verify(blogPage);
        }

        [Fact]
        public async Task Test_ArchivePage()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            ArchivePage archivePage = new ArchivePage(page);
            await archivePage.NavigateAsync();
            Dictionary<string, string> headers = await archivePage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("The complete archive of blog posts · Kaylumah");

            await HtmlPageVerifier.Verify(archivePage);
        }

        [Fact]
        public async Task Test_NotFoundPage()
        {
            IPage page = await _PlaywrightFixture.GetPage();
            NotFoundPage notFoundPage = new NotFoundPage(page);
            await notFoundPage.NavigateAsync();
            Dictionary<string, string> headers = await notFoundPage.GetHeaders();
            string title = await page.TitleAsync();
            title.Should().Be("Page not found · Kaylumah");

            await HtmlPageVerifier.Verify(notFoundPage);
        }

        [Theory]
        [MemberData(nameof(GetBlogPages))]
        public async Task Test_BlogPages(string path)
        {
            IPage blogPage = await _PlaywrightFixture.GetPage();
            BlogItemPage blogItemPage = new BlogItemPage(path, blogPage);
            await blogItemPage.NavigateAsync();
            string testParameter = path
                .Replace("/", "_")
                .Replace(".html", "");
            string methodName = $"{nameof(Test_BlogPages)}_{testParameter}";
            await HtmlPageVerifier.Verify(blogItemPage, methodName);
        }

        public static IEnumerable<object[]> GetBlogPages()
        {
            yield return new object[] { "2023/04/14/csharp-client-for-openapi-revistted.html" };
            yield return new object[] { "2022/09/17/how-to-use-azurite-for-testing-azure-storage-in-dotnet.html" };
            yield return new object[] { "2022/06/07/share-debug-configuration-with-launch-profiles.html" };
            yield return new object[] { "2022/02/21/working-with-azure-sdk-for-dotnet.html" };
            yield return new object[] { "2022/01/31/improve-code-quality-with-bannedsymbolanalyzers.html" };
            yield return new object[] { "2021/11/29/validated-strongly-typed-ioptions.html" };
            yield return new object[] { "2021/11/14/capture-logs-in-unit-tests.html" };
            yield return new object[] { "2021/07/17/decreasing-solution-build-time-with-filters.html" };
            yield return new object[] { "2021/05/23/generate-csharp-client-for-openapi.html" };
            yield return new object[] { "2021/04/11/an-approach-to-writing-mocks.html" };
            yield return new object[] { "2021/03/27/set-nuget-metadata-via-msbuild.html" };
            yield return new object[] { "2019/09/07/using-csharp-code-your-git-hooks.html" };
        }
    }
}
