// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;

namespace Test.Specflow.Utilities
{
    public static class ByteExtensions
    {
        public static string GetString(this byte[] bytes)
        {
            return bytes.GetString(new UTF8Encoding(false));
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

        public static SyndicationFeed ToSyndicationFeed(this byte[] bytes)
        {
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            return feed;
        }

        public static SiteMap ToSiteMap(this byte[] bytes)
        {
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            XmlDocument document = new XmlDocument();
            document.Load(xmlReader);
            XmlNode root = document.DocumentElement?.SelectSingleNode("//*[local-name()='urlset']");
            XmlNodeList children = root?.SelectNodes("//*[local-name()='url']");

            List<SiteMapNode> nodes = new List<SiteMapNode>();
            foreach (XmlNode child in children)
            {
                string location = child.SelectSingleNode("//*[local-name()='loc']")?.InnerText;
                nodes.Add(new SiteMapNode()
                {
                    Url = location
                });
            }

            SiteMap sitemap = new SiteMap()
            {
                Items = nodes
            };

            return sitemap;
        }
    }
}
