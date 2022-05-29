// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public class SiteMapGenerator
{
    private readonly ILogger _logger;
    public SiteMapGenerator(ILogger<SiteMapGenerator> logger)
    {
        _logger = logger;
    }

    public SiteMap Create(SiteMetaData siteMetaData)
    {
        _logger.LogInformation("Generate SiteMap");

        var pages = siteMetaData.Pages
                        .Where(file => ".html".Equals(Path.GetExtension(file.Name), StringComparison.Ordinal))
                        .Where(file => !"404.html".Equals(file.Name, StringComparison.Ordinal))
                        .ToList();

        var siteMapNodes = new List<SiteMapNode>();
        foreach(var page in pages)
        {
            siteMapNodes.Add(new SiteMapNode
            { 
                Url = GlobalFunctions.AbsoluteUrl(page.Url),
                LastModified = page.LastModified
            });
        }

        var siteMap = new SiteMap
        {
            Items = siteMapNodes
        };
        return siteMap;

        /*
        var settings = new XmlWriterSettings()
        {
            Indent = true,
            NamespaceHandling = NamespaceHandling.Default
        };
        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);

        var ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        writer.WriteStartDocument();
        writer.WriteStartElement("urlset", ns);
        */
        /*
        foreach (var page in pages)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", GlobalFunctions.AbsoluteUrl(page.Url));
            // writer.WriteElementString("changefreq", "weekly");
            // writer.WriteElementString("priority", "1.0");
            writer.WriteEndElement();
        }
        */

        // writer.Close();
        // return Encoding.UTF8.GetString(stream.ToArray());
    }
}
