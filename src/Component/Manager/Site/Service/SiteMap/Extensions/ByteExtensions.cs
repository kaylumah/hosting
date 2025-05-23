﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;

namespace System
{
    public static partial class ByteExtensions
    {
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
