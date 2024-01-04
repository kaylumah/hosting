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

    public abstract class BasePageObject
    {
        public abstract string PagePath { get; }

        readonly IBrowser _Browser;
        protected IPage Page { get; private set; }
        public BasePageObject(IBrowser browser)
        {
            _Browser = browser;
        }

        public async Task NavigateAsync ()
        {
            Page = await _Browser.NewPageAsync();
            Page.Console += (_, msg) => {

            };

            string baseUrl = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            
            List<IResponse> responses = new List<IResponse>();
            Page.Response += (_, response) => {
                responses.Add(response);
            };

            await Page.GotoAsync($"{baseUrl}/{PagePath}");
            string test = await Page.ContentAsync();
            IBrowserContext context = Page.Context;
            System.Collections.Generic.IReadOnlyList<IFrame> frames = Page.Frames;
            string url = Page.Url;
            IAPIRequestContext apiRequest = Page.APIRequest;
            string title = await Page.TitleAsync();
        }
    }

    public class AtomFeed : BasePageObject
    {
        public AtomFeed(IBrowser browser) : base(browser)
        {
        }

        public override string PagePath => "about.html"; //"feed.xml";
    }

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
            bool hasContentHeader = response.Headers.TryGetValue("content-type", out string contentType);
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

    [TestClass]
    public class UnitTest3 : BrowserTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            IBrowserContext context = await NewContextAsync(new BrowserNewContextOptions() {
                BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl"
            });
            AtomFeed atomFeed = new AtomFeed(context.Browser);
            await atomFeed.NavigateAsync();
        }
    }
}
