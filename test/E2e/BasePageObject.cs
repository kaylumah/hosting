// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Playwright;

#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3003 // Type is not CLS-compliant
namespace Test.E2e
{
    public abstract class BasePageObject
    {
        public abstract string PagePath { get; }

        readonly IPage _Page;

        public IResponse PageResponse { get; private set; }
        public List<IResponse> Responses { get; } = new List<IResponse>();

        public BasePageObject(IPage page)
        {
            _Page = page;
        }

        public Task<Dictionary<string, string>> GetHeaders()
        {
            return PageResponse.AllHeadersAsync();
        }

        public Task<string> GetContent()
        {
            return PageResponse.TextAsync();
        }

        public async Task NavigateAsync()
        {
            _Page.Response += Page_Response;

            await _Page.GotoAsync(PagePath);
        }

        void Page_Response(object sender, IResponse e)
        {
            if (e.Url.EndsWith(PagePath, StringComparison.Ordinal))
            {
                PageResponse = e;
            }
            else
            {
                Responses.Add(e);
            }
        }
    }

    public abstract class XmlPage : BasePageObject
    {
        protected XmlPage(IPage page) : base(page)
        {
        }
    }

    public abstract class HtmlPage : BasePageObject
    {
        protected HtmlPage(IPage page) : base(page)
        {
        }
    }

    public class AtomFeedPage : XmlPage
    {
        public AtomFeedPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "feed.xml";
    }

    public class SitemapPage : XmlPage
    {
        public SitemapPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "sitemap.xml";
    }

    public class AboutPage : HtmlPage
    {
        public AboutPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "about.html";
    }

}
