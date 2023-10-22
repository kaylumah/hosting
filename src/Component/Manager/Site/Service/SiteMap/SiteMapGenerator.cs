// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public partial class SiteMapGenerator
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Generate SiteMap")]
    private partial void LogGenerateSiteMap();

    private readonly ILogger _logger;
    public SiteMapGenerator(ILogger<SiteMapGenerator> logger)
    {
        _logger = logger;
    }

    public SiteMap Create(SiteMetaData siteMetaData)
    {
        LogGenerateSiteMap();

        List<PageMetaData> pages = siteMetaData.Pages
                        .Where(file => ".html".Equals(Path.GetExtension(file.Name), StringComparison.Ordinal))
                        .Where(file => !"404.html".Equals(file.Name, StringComparison.Ordinal))
                        .ToList();

        List<SiteMapNode> siteMapNodes = new List<SiteMapNode>();
        foreach (PageMetaData page in pages)
        {
            SiteMapNode node = new SiteMapNode
            {
                Url = GlobalFunctions.AbsoluteUrl(page.Uri),
                LastModified = page.Modified
            };

            if (page.Name.Equals("index.html", StringComparison.OrdinalIgnoreCase))
            {
                node.Url = GlobalFunctions.Url.Value;
            }

            siteMapNodes.Add(node);
        }

        SiteMap siteMap = new SiteMap
        {
            Items = siteMapNodes
        };
        return siteMap;
    }
}
