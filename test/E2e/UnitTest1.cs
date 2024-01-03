// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS3009 // Base type is not CLS-compliant
#pragma warning disable CS3002 // Return type is not CLS-compliant
namespace Test.E2e
{
    [TestClass]
    public class UnitTest1 : PlaywrightTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            APIRequestNewContextOptions options = new APIRequestNewContextOptions()
            {
                BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl"
            };
            IAPIRequestContext context = await Playwright.APIRequest.NewContextAsync(options);
            IAPIResponse response = await context.GetAsync("feed.xml");
            await Expect(response).ToBeOKAsync();
        }
    }

    [TestClass]
    public class UnitTest2 : PageTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            await Page.GotoAsync("feed.xml");
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            BrowserNewContextOptions browserNewContextOptions = base.ContextOptions();
            browserNewContextOptions.BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            return browserNewContextOptions;
        }
    }

    // [TestClass]
    // public class UnitTest3 : BrowserTest
    // {
    //     [TestMethod]
    //     public async Task TestMethod1()
    //     {
    //         //await Page.GotoAsync("feed.xml");
    //     }
    // }
}
