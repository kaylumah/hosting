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
            _Page.Close += Page_Close;
            _Page.Console += Page_Console;
            _Page.Crash += Page_Crash;
            // _Page.Dialog += Page_Dialog;
            // _Page.DOMContentLoaded += Page_DOMContentLoaded;
            // _Page.Download += Page_Download;
            // _Page.FileChooser += Page_FileChooser;
            // _Page.FrameAttached += Page_FrameAttached;
            // _Page.FrameDetached += Page_FrameDetached;
            // _Page.FrameNavigated += Page_FrameNavigated;
            _Page.Load += Page_Load;
            _Page.PageError += Page_PageError;
            // _Page.Popup += Page_Popup;
            _Page.Request += Page_Request;
            _Page.RequestFailed += Page_RequestFailed;
            _Page.RequestFinished += Page_RequestFinished;
            _Page.Response += Page_Response;
            // _Page.WebSocket += Page_WebSocket;
            // _Page.Worker += Page_Worker;

            string baseUrl = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";

            //List<IResponse> responses = new List<IResponse>();
            //_Page.Response += (_, response) => {
            //    responses.Add(response);
            //};

            await _Page.GotoAsync($"{baseUrl}/{PagePath}");
            //string test = await _Page.ContentAsync();
            //IBrowserContext context = _Page.Context;
            //System.Collections.Generic.IReadOnlyList<IFrame> frames = _Page.Frames;
            //string url = _Page.Url;
            //IAPIRequestContext apiRequest = _Page.APIRequest;
            //string title = await _Page.TitleAsync();
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
