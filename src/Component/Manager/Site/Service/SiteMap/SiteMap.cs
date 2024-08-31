// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap
{
    public class SiteMapArtifact
    {
        public string FileName
        { get; set; }

        public SiteMap SiteMap
        { get; set; }

        public SiteMapArtifact(string fileName, SiteMap siteMap)
        {
            FileName = fileName;
            SiteMap = siteMap;
        }
    }

    public class SiteMap
    {
        public DateTimeOffset? LastModified
        { get; set; }

        public IEnumerable<SiteMapNode> Items
        { get; set; }

        public SiteMap(IEnumerable<SiteMapNode> items)
        {
            IEnumerable<SiteMapNode> orderedByLocation = items.OrderBy(node => node.Url);
            Items = orderedByLocation;

            IEnumerable<SiteMapNode> orderedByModified = items.OrderByDescending(node => node.LastModified);
            SiteMapNode? mostRecent = orderedByModified.FirstOrDefault();
            LastModified = mostRecent?.LastModified;
        }

        public SiteMapFormatter GetFormatter() => new SiteMapFormatter(this);

        public void SaveAsXml(XmlWriter writer)
        {
            GetFormatter().WriteXml(writer);
        }
    }

    public class SiteMapNode
    {
        public SitemapFrequency? Frequency
        { get; set; }
        public DateTimeOffset? LastModified
        { get; set; }
        public double? Priority
        { get; set; }
        public string Url
        { get; set; } = null!;
    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }

    public class SiteMapFormatter
    {
        readonly SiteMap _SiteMap;

        public SiteMapFormatter(SiteMap siteMap)
        {
            _SiteMap = siteMap;
        }

        public void WriteXml(XmlWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);

            writer.WriteStartElement(Constants.UrlSetTag, Constants.SiteMapNamespace);
            WriteItems(writer, _SiteMap.Items);
        }

        static void WriteItem(XmlWriter writer, SiteMapNode item)
        {
            writer.WriteStartElement(Constants.UrlTag);
            WriteItemContents(writer, item);
            writer.WriteEndElement();
        }

        static void WriteItemContents(XmlWriter writer, SiteMapNode item)
        {
            writer.WriteElementString(Constants.LocationTag, item.Url);
            if (item.LastModified.HasValue)
            {
                string formatted = item.LastModified.GetValueOrDefault().ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture);
                writer.WriteElementString(Constants.LastModifiedTag, formatted);
            }
        }

        static void WriteItems(XmlWriter writer, IEnumerable<SiteMapNode> items)
        {
            foreach (SiteMapNode item in items)
            {
                WriteItem(writer, item);
            }
        }
    }
}
