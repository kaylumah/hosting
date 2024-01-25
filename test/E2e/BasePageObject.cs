// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Playwright;

#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3003 // Type is not CLS-compliant
#pragma warning disable CS3008 // Identifier is not CLS-compliant
namespace Test.E2e
{
    public abstract class BasePageObject
    {
        public abstract string PagePath { get; }

        protected readonly IPage _Page;

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
            bool isRedirect = e.Status == 301;
            bool isTargetUrl = e.Url.EndsWith(PagePath, StringComparison.Ordinal);
            bool matchWithoutRedirect = isRedirect == false && isTargetUrl;

            bool matchWithRedirect = e.Request.RedirectedFrom?.Url.EndsWith(PagePath, StringComparison.Ordinal) ?? false;

            bool isPageResponse = matchWithoutRedirect || matchWithRedirect;
            if (isPageResponse)
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

        public async Task<Dictionary<string, string>> GetMetaTags()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            ILocator metaTagNameLocator = _Page.Locator("//meta[@name]");
            IReadOnlyList<ILocator> metaNameTags = await metaTagNameLocator.AllAsync();
            foreach (ILocator metaTag in metaNameTags)
            {
                string key = await metaTag.GetAttributeAsync("name");
                string value = await metaTag.GetAttributeAsync("content");
                result.Add(key, value);
            }

            ILocator metaTagPropertyLocator = _Page.Locator("//meta[@property]");
            IReadOnlyList<ILocator> metaPropertyTags = await metaTagPropertyLocator.AllAsync();
            foreach (ILocator metaTag in metaPropertyTags)
            {
                string key = await metaTag.GetAttributeAsync("property");
                string value = await metaTag.GetAttributeAsync("content");
                result.Add(key, value);
            }

            return result;
        }
    }

    public abstract class TxtPage : BasePageObject
    {
        protected TxtPage(IPage page) : base(page)
        {
        }
    }

    public class RobotsPage : TxtPage
    {
        public RobotsPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "robots.txt";
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

    public class HomePage : HtmlPage
    {
        public HomePage(IPage page) : base(page)
        {
        }

        public override string PagePath => "index.html";
    }

    public class AboutPage : HtmlPage
    {
        public AboutPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "about.html";
    }
    public class NotFoundPage : HtmlPage
    {
        public NotFoundPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "404.html";
    }

    public class ArchivePage : HtmlPage
    {
        public ArchivePage(IPage page) : base(page)
        {
        }

        public override string PagePath => "archive.html";
    }

    public class BlogPage : HtmlPage
    {
        public BlogPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "blog.html";
    }

    public class BlogItemPage : HtmlPage
    {
        readonly string _PagePath;
        public BlogItemPage(string pagePath, IPage page) : base(page)
        {
            _PagePath = pagePath;
        }

        public override string PagePath => _PagePath;
    }

}
