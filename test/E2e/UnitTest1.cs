// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
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
            AtomFeed atomFeed = new AtomFeed(Page);
            await atomFeed.NavigateAsync();

            Page.Url.Should().EndWith(atomFeed.PagePath);

            Dictionary<string, string> headers = await atomFeed.GetHeaders();
            
            byte[] bytes = await atomFeed.PageResponse.BodyAsync();
            // TODO this is also in Specflow proj
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            feed.Title.Text.Should().Be("Max Hamulyák · Kaylumah");
        }

        [TestMethod]
        public async Task Test_Sitemap()
        {
            Sitemap sitemap = new Sitemap(Page);
            await sitemap.NavigateAsync();

            Page.Url.Should().EndWith(sitemap.PagePath);

            Dictionary<string, string> headers = await sitemap.GetHeaders();

            byte[] bytes = await sitemap.PageResponse.BodyAsync();
            // TODO this is also in Specflow proj
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            feed.Title.Text.Should().Be("Max Hamulyák · Kaylumah");
        }

        [TestMethod]
        public async Task Test_AboutPage()
        {
            AboutPage aboutPage = new AboutPage(Page);
            await aboutPage.NavigateAsync();
            Dictionary<string, string> headers = await aboutPage.GetHeaders();
            string text = await aboutPage.GetContent();
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            BrowserNewContextOptions browserNewContextOptions = base.ContextOptions();
            browserNewContextOptions.BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            return browserNewContextOptions;
        }
    }
}
