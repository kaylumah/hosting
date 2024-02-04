// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public class RenderData
    {
        public SiteMetaData Site
        { get; set; }
        public PageMetaData Page
        { get; set; }
        public string Content => Page?.Content ?? string.Empty;
        public string Title => Page?.Title ?? Site?.Title ?? string.Empty;
        public string Description => Page?.Description ?? Site?.Description ?? string.Empty;
        public string Language => Page?.Language ?? Site?.Language ?? string.Empty;
        public string Author => Page?.Author ?? Site?.Author ?? string.Empty;
        public string Url => Page?.Uri ?? Site?.Url ?? string.Empty;

        public RenderData(SiteMetaData siteMetaData, PageMetaData pageMetaData)
        {
            Site = siteMetaData;
            Page = pageMetaData;
        }
    }
}
