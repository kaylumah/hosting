// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public class RenderData
    {
        public SiteMetaData Site
        { get; set; }
        public PageMetaData Page
        { get; set; }
        public string Content => GetContent();
        public string Title => GetTitle();
        public string Description => GetDescription();
        public string Language => GetLanguage();
        public string Author => GetAuthor();
        public string Url => GetUrl();

        public RenderData(SiteMetaData siteMetaData, PageMetaData pageMetaData)
        {
            Site = siteMetaData;
            Page = pageMetaData;
        }

        string GetContent()
        {
            string content = Page?.Content ?? string.Empty;
            return content;
        }

        string GetTitle()
        {
            string title = Page?.Title ?? Site?.Title ?? string.Empty;
            return title;
        }

        string GetDescription()
        {
            string description = Page?.Description ?? Site?.Description ?? string.Empty;
            return description;
        }

        string GetLanguage()
        {
            string language = Page?.Language ?? Site?.Language ?? string.Empty;
            return language;
        }

        string GetAuthor()
        {
            string author = Page?.Author ?? Site?.Author ?? string.Empty;
            return author;
        }

        string GetUrl()
        {
            string uri = Page?.Uri ?? Site?.Url ?? string.Empty;
            return uri;
        }
    }

    public static class RenderDataExtensions
    {
        public static readonly Func<RenderData, bool> Html;

        static RenderDataExtensions()
        {
            Html = (renderData) => renderData.IsHtml();
        }

        public static bool IsHtml(this RenderData renderData)
        {
            bool result = PageMetaDataExtensions.Html(renderData.Page);
            return result;
        }
    }
}
