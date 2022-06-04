// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public partial class SiteMapGenerator
{
    private readonly ILogger _logger;
    public SiteMapGenerator(ILogger<SiteMapGenerator> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Information,
            Message = "Generate SiteMap")]
        public partial void LogGenerateSiteMap();

    public SiteMap Create(SiteMetaData siteMetaData)
    {
        LogGenerateSiteMap();

        var pages = siteMetaData.Pages
                        .Where(file => ".html".Equals(Path.GetExtension(file.Name), StringComparison.Ordinal))
                        .Where(file => !"404.html".Equals(file.Name, StringComparison.Ordinal))
                        .ToList();

        var siteMapNodes = new List<SiteMapNode>();
        foreach (var page in pages)
        {
            var node = new SiteMapNode
            {
                Url = GlobalFunctions.AbsoluteUrl(page.Url),
                LastModified = page.LastModified
            };

            if (page.Name.Equals("index.html", StringComparison.OrdinalIgnoreCase))
            {
                node.Url = GlobalFunctions.Instance.Url;
            }

            siteMapNodes.Add(node);
        }

        var siteMap = new SiteMap
        {
            Items = siteMapNodes
        };
        return siteMap;
    }
}
