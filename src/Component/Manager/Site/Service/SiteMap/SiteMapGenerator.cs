// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
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

    public string Create(SiteMetaData siteMetaData)
    {
        _logger.LogInformation("Generate SiteMap");

        var pages = siteMetaData.Pages
                        .Where(file => ".html".Equals(Path.GetExtension(file.Name), StringComparison.Ordinal))
                        .Where(file => !"404.html".Equals(file.Name, StringComparison.Ordinal))
                        .ToList();


            /*
         site.Pages = pages
                .Where(file => ".html".Equals(Path.GetExtension(file.Name), StringComparison.Ordinal))
                .Where(file => !"404.html".Equals(file.Name, StringComparison.Ordinal))
                .Select(x => new
                {
                    Url = x.Url,
                    x.LastModified,
                    Sitemap = x.Sitemap
                });*/

        var settings = new XmlWriterSettings()
        {
            Indent = true,
            NamespaceHandling = NamespaceHandling.Default
        };
        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);
        // https://github.com/microsoft/referencesource/blob/master/System.ServiceModel/System/ServiceModel/Syndication/Atom10FeedFormatter.cs
        // https://social.msdn.microsoft.com/Forums/en-US/b0c0d3fe-f3da-4102-9d6b-b1cdc80065bf/how-to-generate-sitemap-for-a-site-to-list-all-the-web-pages?forum=aspgettingstarted
        // https://rehansaeed.com/dynamically-generating-sitemap-xml-for-asp-net-mvc/
        // https://stackoverflow.com/questions/55504758/how-to-write-sitemap-file-with-xmlwriter
        var ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        writer.WriteStartDocument();
        writer.WriteStartElement("urlset", ns);

        foreach(var page in pages)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", GlobalFunctions.AbsoluteUrl(page.Url));
            // writer.WriteElementString("changefreq", "weekly");
            // writer.WriteElementString("priority", "1.0");
            writer.WriteEndElement();
        }

        writer.Close();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
