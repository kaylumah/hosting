// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Playwright;

#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3003 // Type is not CLS-compliant
#pragma warning disable CS3008 // Identifier is not CLS-compliant
namespace Test.E2e
{
    public abstract class BasePageObject
    {
        public abstract string PagePath
        { get; }

        protected readonly IPage Page;

        IResponse? PageResponse
        { get; set; }
        List<IResponse> Responses
        { get; } = new List<IResponse>();

        public BasePageObject(IPage page)
        {
            Page = page;
        }

        public async Task<byte[]> ScreenshotAsync()
        {
            PageScreenshotOptions pageScreenshotOptions = new PageScreenshotOptions();
            pageScreenshotOptions.FullPage = true;
            byte[] bytes = await Page.ScreenshotAsync(pageScreenshotOptions);
            return bytes;
        }

        public async Task<Dictionary<string, string>?> GetHeaders()
        {
            if (PageResponse == null)
            {
                return null;
            }

            Dictionary<string, string> result = await PageResponse.AllHeadersAsync();
            return result;
        }

        public async Task<string?> GetContent()
        {
            if (PageResponse == null)
            {
                return null;
            }

            string result = await PageResponse.TextAsync();
            return result;
        }

        public async Task NavigateAsync()
        {
            Page.Response += Page_Response;

            await Page.GotoAsync(PagePath);
        }

        void Page_Response(object? sender, IResponse e)
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

        public async Task<Dictionary<string, string?>> GetMetaTags()
        {
            Dictionary<string, string?> result = new Dictionary<string, string?>();
            ILocator metaTagNameLocator = Page.Locator("//meta[@name]");
            IReadOnlyList<ILocator> metaNameTags = await metaTagNameLocator.AllAsync();
            foreach (ILocator metaTag in metaNameTags)
            {
                string? key = await metaTag.GetAttributeAsync("name");
                string? value = await metaTag.GetAttributeAsync("content");
                if (key != null)
                {
                    result.TryAdd(key, value);
                }
            }

            ILocator metaTagPropertyLocator = Page.Locator("//meta[@property]");
            IReadOnlyList<ILocator> metaPropertyTags = await metaTagPropertyLocator.AllAsync();
            foreach (ILocator metaTag in metaPropertyTags)
            {
                string? key = await metaTag.GetAttributeAsync("property");
                string? value = await metaTag.GetAttributeAsync("content");
                if (key != null)
                {
                    result.TryAdd(key, value);
                }
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
        public AtomFeedPage(IPage page, string feedUrl = "feed.xml") : base(page)
        {
            PagePath = feedUrl;
        }

        public override string PagePath
        { get; }
    }

    public class SitemapIndexPage : XmlPage
    {
        public SitemapIndexPage(IPage page, string sitemapUrl = "sitemap_index.xml") : base(page)
        {
            PagePath = sitemapUrl;
        }

        public override string PagePath
        { get; }
    }

    public class SitemapPage : XmlPage
    {
        public SitemapPage(IPage page, string sitemapUrl = "sitemap.xml") : base(page)
        {
            PagePath = sitemapUrl;
        }

        public override string PagePath
        { get; }
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

    public class PrivacyPage : HtmlPage
    {
        public PrivacyPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "privacy.html";
    }

    public class NotFoundPage : HtmlPage
    {
        public NotFoundPage(IPage page) : base(page)
        {
        }

        public override string PagePath => "404.html";
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
        public BlogItemPage(string pagePath, IPage page) : base(page)
        {
            PagePath = pagePath;
        }

        public override string PagePath
        { get; }
    }
}
