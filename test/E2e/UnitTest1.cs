// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

            Page.Close += Page_Close;
            Page.Console += Page_Console;
            Page.Crash += Page_Crash;
            // Page.Dialog += Page_Dialog;
            // Page.DOMContentLoaded += Page_DOMContentLoaded;
            // Page.Download += Page_Download;
            // Page.FileChooser += Page_FileChooser;
            // Page.FrameAttached += Page_FrameAttached;
            // Page.FrameDetached += Page_FrameDetached;
            // Page.FrameNavigated += Page_FrameNavigated;
            Page.Load += Page_Load;
            Page.PageError += Page_PageError;
            // Page.Popup += Page_Popup;
            Page.Request += Page_Request;
            Page.RequestFailed += Page_RequestFailed;
            Page.RequestFinished += Page_RequestFinished;
            Page.Response += Page_Response;
            // Page.WebSocket += Page_WebSocket;
            // Page.Worker += Page_Worker;

            string baseUrl = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            
            //List<IResponse> responses = new List<IResponse>();
            //Page.Response += (_, response) => {
            //    responses.Add(response);
            //};

            await Page.GotoAsync($"{baseUrl}/{PagePath}");
            //string test = await Page.ContentAsync();
            //IBrowserContext context = Page.Context;
            //System.Collections.Generic.IReadOnlyList<IFrame> frames = Page.Frames;
            //string url = Page.Url;
            //IAPIRequestContext apiRequest = Page.APIRequest;
            //string title = await Page.TitleAsync();
        }

        private void Page_PageError(object sender, string e)
        {
            throw new NotImplementedException();
        }

        private void Page_Crash(object sender, IPage e)
        {
            throw new NotImplementedException();
        }

        private void Page_Console(object sender, IConsoleMessage e)
        {
            // Only for HTML
        }

        private void Page_Close(object sender, IPage e)
        {
            // throw new NotImplementedException();
        }

        private void Page_RequestFailed(object sender, IRequest e)
        {
            throw new NotImplementedException();
        }

        private void Page_Request(object sender, IRequest e)
        {
            string message = $"IRequest => {e.Method} {e.Url} {e.ResourceType} {e.RedirectedFrom}";
            Log(message);
        }

        private void Page_Response(object sender, IResponse e)
        {
            string message = $"IResponse => (header) {e.Status} {e.Url}";
            Log(message);
        }

        private void Page_RequestFinished(object sender, IRequest e)
        {
            string message = $"IRequest => {e.Method} {e.Url} {e.ResourceType} {e.RedirectedFrom}";
            Log(message);
        }

        private void Page_Load(object sender, IPage e)
        {
            // 6
        }

        public void Log(string message, [CallerMemberName] string writer = "none")
        {
            string logMessage = $"[{writer}]: {message}";
            Console.WriteLine(logMessage);
        }
    }

    public class AtomFeed : BasePageObject
    {
        public AtomFeed(IBrowser browser) : base(browser)
        {
        }

        public override string PagePath => "feed.xml";
    }

    public class AboutPage : BasePageObject
    {
        public AboutPage(IBrowser browser) : base(browser)
        {
        }

        public override string PagePath => "about.html"; 
    }

    [TestClass]
    public class UnitTest1 : PlaywrightTest
    {
        //[TestMethod]
        //public async Task TestMethod1()
        //{
        //    APIRequestNewContextOptions options = new APIRequestNewContextOptions()
        //    {
        //        BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl"
        //    };
        //    IAPIRequestContext context = await Playwright.APIRequest.NewContextAsync(options);
        //    IAPIResponse response = await context.GetAsync("feed.xml");
        //    await Expect(response).ToBeOKAsync();
        //    bool hasContentHeader = response.Headers.TryGetValue("content-type", out string contentType);
        //}
    }

    [TestClass]
    public class UnitTest2 : PageTest
    {
        //[TestMethod]
        //public async Task TestMethod1()
        //{
        //    await Page.GotoAsync("feed.xml");
        //}

        //public override BrowserNewContextOptions ContextOptions()
        //{
        //    BrowserNewContextOptions browserNewContextOptions = base.ContextOptions();
        //    browserNewContextOptions.BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
        //    return browserNewContextOptions;
        //}
    }

    [TestClass]
    public class UnitTest3 : BrowserTest
    {
        async Task<IBrowserContext> BuildContext()
        {
            IBrowserContext context = await NewContextAsync(new BrowserNewContextOptions()
            {
                BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl"
            });
            return context;
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            IBrowserContext context = await BuildContext();
            AtomFeed atomFeed = new AtomFeed(context.Browser);
            await atomFeed.NavigateAsync();
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            IBrowserContext context = await BuildContext();
            AboutPage aboutPage = new AboutPage(context.Browser);
            await aboutPage.NavigateAsync();
        }
    }
}
