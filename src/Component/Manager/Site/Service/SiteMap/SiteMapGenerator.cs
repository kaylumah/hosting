// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
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

            List<PageMetaData> pages = siteMetaData.Pages
                            .Where(file =>
                            {
                                string extension = Path.GetExtension(file.Name);
                                bool isHtml = ".html".Equals(extension, StringComparison.Ordinal);
                                return isHtml;
                            })
                            .Where(file => !"404.html".Equals(file.Name, StringComparison.Ordinal))
                            .ToList();

            List<SiteMapNode> siteMapNodes = new List<SiteMapNode>();
            foreach (PageMetaData page in pages)
            {
                SiteMapNode node = new SiteMapNode();
                node.Url = GlobalFunctions.AbsoluteUrl(page.Uri);
                node.LastModified = page.Modified;

                if (page.Name.Equals("index.html", StringComparison.OrdinalIgnoreCase))
                {
                    node.Url = GlobalFunctions.Url.Value!;
                }

                siteMapNodes.Add(node);
            }

            SiteMap siteMap = new SiteMap();
            siteMap.Items = siteMapNodes;
            return siteMap;
        }
    }
}
