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
        public BasePage Page
        { get; set; }
        public string Content => GetContent();
        public string Title => GetTitle();
        public string Description => GetDescription();
        public string Language => GetLanguage();
        public string Author => GetAuthor();
        public string Url => GetUrl();

        public RenderData(SiteMetaData siteMetaData, BasePage pageMetaData)
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
            if (Page is PageMetaData pageMetaData)
            {
                string title = pageMetaData?.Title ?? Site?.Title ?? string.Empty;
                return title;
            }

            return string.Empty;
        }

        string GetDescription()
        {
            if (Page is PageMetaData pageMetaData)
            {
                string description = pageMetaData?.Description ?? Site?.Description ?? string.Empty;
                return description;
            }

            return string.Empty;
        }

        string GetLanguage()
        {
            if (Page is PageMetaData pageMetaData)
            {
                string language = pageMetaData?.Language ?? Site?.Language ?? string.Empty;
                return language;
            }

            return string.Empty;
        }

        string GetAuthor()
        {
            if (Page is PageMetaData pageMetaData)
            {
                string author = pageMetaData?.Author ?? Site?.Author ?? string.Empty;
                return author;
            }

            return string.Empty;
        }

        string GetUrl()
        {
            // TODO consider if we can do it here...
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
            if (renderData.Page is PageMetaData pageMetaData)
            {
                bool result = PageMetaDataExtensions.Html((PageMetaData)renderData.Page);
                return result;
            }

            return false;
        }
    }
}
