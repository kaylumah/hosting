// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;

namespace Test.Unit.Utilities
{
    public static class ByteExtensions
    {
        public static string GetString(this byte[] bytes)
        {
            UTF8Encoding encoding = new UTF8Encoding(false);
            string result = bytes.GetString(encoding);
            return result;
        }

        public static string GetString(this byte[] bytes, Encoding encoding)
        {
            string contents = encoding.GetString(bytes);
            return contents;
        }

        public static HtmlDocument ToHtmlDocument(this byte[] bytes)
        {
            string contents = bytes.GetString();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(contents);
            return htmlDoc;
        }

        public static FeedArtifact ToSyndicationFeed(this byte[] bytes, string fileName)
        {
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            FeedArtifact result = new FeedArtifact(fileName, feed);
            return result;
        }

        public static SiteMapArtifact ToSiteMap(this byte[] bytes, string fileName)
        {
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            XmlDocument document = new XmlDocument();
            document.Load(xmlReader);
            XmlNode? root = document.DocumentElement?.SelectSingleNode("//*[local-name()='urlset']");
            XmlNodeList? children = root?.SelectNodes("//*[local-name()='url']");

            List<SiteMapNode> nodes = new List<SiteMapNode>();

            if (children != null)
            {
                foreach (XmlNode child in children)
                {
                    // TODO better solution does not work
                    //                 string location = child.SelectSingleNode("//*[local-name()='loc']")?.InnerText;
                    string? location = child.ChildNodes[0]?.InnerText;
                    string? lastModified = child.ChildNodes[1]?.InnerText;
                    if (location != null)
                    {
                        SiteMapNode siteMapNode = new SiteMapNode();
                        siteMapNode.Url = location;
#pragma warning disable
                        siteMapNode.LastModified = DateTimeOffset.Parse(lastModified);
#pragma warning restore;
                        nodes.Add(siteMapNode);
                    }
                }
            }

            SiteMap siteMap = new SiteMap(nodes);
            SiteMapArtifact result = new SiteMapArtifact(fileName, siteMap);
            return result;
        }
    }
}
