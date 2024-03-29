// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap
{
    public partial class SiteMapGenerator
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Generate SiteMap")]
        private partial void LogGenerateSiteMap();

        readonly ILogger _Logger;
        public SiteMapGenerator(ILogger<SiteMapGenerator> logger)
        {
            _Logger = logger;
        }

        public SiteMap Create(SiteMetaData siteMetaData)
        {
            LogGenerateSiteMap();

            List<PageMetaData> pages = siteMetaData.GetPages()
                            .Where(file => file.IsHtml())
                            .Where(file =>
                            {
                                bool is404 = file.IsUrl("404.html");
                                bool result = is404 == false;
                                return result;
                            })
                            .ToList();

            List<SiteMapNode> siteMapNodes = new List<SiteMapNode>();
            foreach (PageMetaData page in pages)
            {
                SiteMapNode node = new SiteMapNode();
                Uri siteMapUri = GlobalFunctions.AbsoluteUri(page.Uri);
                node.Url = siteMapUri.ToString();
                node.LastModified = page.Modified;

                bool isIndex = page.IsUrl("index.html");
                if (isIndex)
                {
                    node.Url = GlobalFunctions.Url.Value!;
                }

                siteMapNodes.Add(node);
            }

            IEnumerable<SiteMapNode> orderedNodes = siteMapNodes.OrderBy(node => node.Url);
            List<SiteMapNode> result = orderedNodes.ToList();

            SiteMap siteMap = new SiteMap();
            siteMap.Items = result;
            return siteMap;
        }
    }
}
