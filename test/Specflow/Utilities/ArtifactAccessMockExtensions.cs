// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;

namespace Test.Specflow.Utilities;

public static class ArtifactAccessMockExtensions
{
    public static byte[] GetArtifactContents(this ArtifactAccessMock artifactAccess, string path)
    {
        var bytes = artifactAccess
            .Artifacts
            .SingleOrDefault(x => path.Equals(x.Path))?.Contents ?? Array.Empty<byte>();
        return bytes;
    }

    public static HtmlDocument GetHtmlDocument(this ArtifactAccessMock artifactAccess, string path)
    {
        var htmlBytes = artifactAccess.GetArtifactContents(path);
        var contents = new UTF8Encoding(false).GetString(htmlBytes);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(contents);
        return htmlDoc;
    }

    public static SyndicationFeed GetFeedArtifact(this ArtifactAccessMock artifactAccess, string path = "feed.xml")
    {
        var atomFeedXmlBytes = artifactAccess.GetArtifactContents(path);
        using var stream = new MemoryStream(atomFeedXmlBytes);
        using var xmlReader = XmlReader.Create(stream);
        var feed = SyndicationFeed.Load(xmlReader);
        return feed;
    }

    public static SiteMap GetSiteMapArtifact(this ArtifactAccessMock artifactAccess, string path = "sitemap.xml")
    {
        var siteMapXmlBytes = artifactAccess.GetArtifactContents(path);
        using var stream = new MemoryStream(siteMapXmlBytes);
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
