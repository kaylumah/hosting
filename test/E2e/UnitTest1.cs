// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3003 // Type is not CLS-compliant
#pragma warning disable CS3009 // Base type is not CLS-compliant
#pragma warning disable CS3002 // Return type is not CLS-compliant
namespace Test.E2e
{
    [TestClass]
    public class UnitTest3 : PageTest
    {
        [TestMethod]
        public async Task Test_AtomFeed()
        {
            AtomFeedPage atomFeed = new AtomFeedPage(Page);
            await atomFeed.NavigateAsync();

            Page.Url.Should().EndWith(atomFeed.PagePath);

            Dictionary<string, string> headers = await atomFeed.GetHeaders();
            
            byte[] bytes = await atomFeed.PageResponse.BodyAsync();
            SyndicationFeed feed = bytes.ToSyndicationFeed();
            feed.Title.Text.Should().Be("Max Hamulyák · Kaylumah");
        }

        [TestMethod]
        public async Task Test_Sitemap()
        {
            SitemapPage sitemapPage = new SitemapPage(Page);
            await sitemapPage.NavigateAsync();

            Page.Url.Should().EndWith(sitemapPage.PagePath);

            Dictionary<string, string> headers = await sitemapPage.GetHeaders();

            byte[] bytes = await sitemapPage.PageResponse.BodyAsync();
            SiteMap sitemap = bytes.ToSiteMap();
            string url = GetBaseUrl();
            sitemap.Items.ToList().ElementAt(0).Url.Should().Be(url);
        }

        [TestMethod]
        public async Task Test_Robots()
        {
            RobotsPage robotsPage = new RobotsPage(Page);
            await robotsPage.NavigateAsync();

            Page.Url.Should().EndWith(robotsPage.PagePath);

            Dictionary<string, string> headers = await robotsPage.GetHeaders();

            byte[] bytes = await robotsPage.PageResponse.BodyAsync();
            UTF8Encoding encoding = new UTF8Encoding(false);
            string robots = encoding.GetString(bytes);
        }

        [TestMethod]
        public async Task Test_AboutPage()
        {
            AboutPage aboutPage = new AboutPage(Page);
            await aboutPage.NavigateAsync();
            Dictionary<string, string> headers = await aboutPage.GetHeaders();
            string title = await Page.TitleAsync();
            title.Should().Be("All about Max Hamulyák from personal to Curriculum Vitae · Kaylumah");
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            BrowserNewContextOptions browserNewContextOptions = base.ContextOptions();
            browserNewContextOptions.BaseURL = GetBaseUrl();
            return browserNewContextOptions;
        }

        string GetBaseUrl()
        {
            string result = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            return result;
        }
    }
}
