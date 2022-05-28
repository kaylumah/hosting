// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public class SiteMapGenerator
{
    private readonly ILogger _logger;

    public SiteMapGenerator(ILogger<SiteMapGenerator> logger)
    {
        _logger = logger;
    }

    private static IEnumerable<PageMetaData> RetrievePostPageMetaDatas(SiteMetaData siteMetaData)
    {
        if (siteMetaData.Collections.TryGetValue("posts", out var posts))
        {
            return posts
                .Where(x => x.Sitemap)
                .ToList();
        }
        return Enumerable.Empty<PageMetaData>();
    }

    public string Create(SiteMetaData siteMetaData)
    {
        _logger.LogInformation("Generate SiteMap");

        var pages = RetrievePostPageMetaDatas(siteMetaData);

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
        writer.Close();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
