// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Playwright;

#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3003 // Type is not CLS-compliant
#pragma warning disable CS3009 // Base type is not CLS-compliant
#pragma warning disable CS3002 // Return type is not CLS-compliant
namespace Test.E2e
{
    public abstract class BasePageObject
    {
        public abstract string PagePath { get; }

        readonly IPage _Page;

        public BasePageObject(IPage page)
        {
            _Page = page;
        }

        public async Task NavigateAsync()
        {
            _Page.Response += Page_Response;

            string baseUrl = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";

            await _Page.GotoAsync($"{baseUrl}/{PagePath}");
            //string test = await _Page.ContentAsync();
            //IBrowserContext context = _Page.Context;
            //System.Collections.Generic.IReadOnlyList<IFrame> frames = _Page.Frames;
            //string url = _Page.Url;
            //IAPIRequestContext apiRequest = _Page.APIRequest;
            //string title = await _Page.TitleAsync();
        }
        void Page_Response(object sender, IResponse e)
        {
            string message = $"IResponse => (header) {e.Status} {e.Url}";
            Log(message);
        }

        public void Log(string message, [CallerMemberName] string writer = "none")
        {
            string logMessage = $"[{writer}]: {message}";
            Console.WriteLine(logMessage);
        }
    }

    public class AtomFeed : BasePageObject
    {
        public AtomFeed(IPage page) : base(page)
        {
        }

        public override string PagePath => "feed.xml";
    }

    public class AboutPage : BasePageObject
    {
        public AboutPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "about.html";
    }

}
