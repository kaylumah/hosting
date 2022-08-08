// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;

namespace Test.Specflow.Utilities;

public static class ByteExtensions
{
    public static string GetStringContent(this byte[] bytes)
    {
        var contents = new UTF8Encoding(false).GetString(bytes);
        return contents;
    }
    
    public static HtmlDocument ToHtmlDocument(this byte[] bytes)
    {
        var contents = bytes.GetStringContent();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(contents);
        return htmlDoc;
    }
    
    public static SyndicationFeed ToSyndicationFeed(this byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var xmlReader = XmlReader.Create(stream);
        var feed = SyndicationFeed.Load(xmlReader);
        return feed;
    }

    public static SiteMap ToSiteMap(this byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var xmlReader = XmlReader.Create(stream);
        var document = new XmlDocument();
        document.Load(xmlReader);
        var root = document.DocumentElement?.SelectSingleNode("//*[local-name()='urlset']");
        var children = root?.SelectNodes("//*[local-name()='url']");

        var nodes = new List<SiteMapNode>();
        foreach (XmlNode child in children)
        {
            var location = child.SelectSingleNode("//*[local-name()='loc']")?.InnerText;
            nodes.Add(new SiteMapNode()
            {
                Url = location
            });
        }
        var sitemap = new SiteMap()
        {
            Items = nodes
        };

        return sitemap;
    }
}
