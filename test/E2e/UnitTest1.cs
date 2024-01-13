// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task TestMethod1()
        {
            AtomFeed atomFeed = new AtomFeed(Page);
            await atomFeed.NavigateAsync();
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            AboutPage aboutPage = new AboutPage(Page);
            await aboutPage.NavigateAsync();
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            BrowserNewContextOptions browserNewContextOptions = base.ContextOptions();
            browserNewContextOptions.BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            return browserNewContextOptions;
        }
    }
}
