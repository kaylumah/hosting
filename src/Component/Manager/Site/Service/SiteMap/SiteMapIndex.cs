// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap
{
    public class SiteMapIndexAsArtifact
    {
        public string FileName
        { get; set; }

        public SiteMapIndex SiteMapIndex
        { get; set; }

        public SiteMapIndexAsArtifact(string fileName, SiteMapIndex siteMapIndex)
        {
            FileName = fileName;
            SiteMapIndex = siteMapIndex;
        }
    }

    public class SiteMapIndex
    {
        public IEnumerable<SiteMapIndexNode> Items
        { get; set; }

        public SiteMapIndex(IEnumerable<SiteMapIndexNode> items)
        {
            IEnumerable<SiteMapIndexNode> orderedByLocation = items.OrderBy(node => node.Url);
            Items = orderedByLocation;
        }

        public SiteMapIndexFormatter GetFormatter() => new SiteMapIndexFormatter(this);

        public void SaveAsXml(XmlWriter writer)
        {
            GetFormatter().WriteXml(writer);
        }
    }

    public class SiteMapIndexNode
    {
        public Uri Url
        { get; set; } = null!;

        public DateTimeOffset? LastModified
        { get; set; }
    }

    public class SiteMapIndexFormatter
    {
        readonly SiteMapIndex _SiteMapIndex;

        public SiteMapIndexFormatter(SiteMapIndex siteMapIndex)
        {
            _SiteMapIndex = siteMapIndex;
        }

        public void WriteXml(XmlWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);

            writer.WriteStartElement(Constants.SiteMapIndexTag, Constants.SiteMapNamespace);
            WriteItems(writer, _SiteMapIndex.Items);
        }

        static void WriteItem(XmlWriter writer, SiteMapIndexNode item)
        {
            writer.WriteStartElement(Constants.SiteMapTag);
            WriteItemContents(writer, item);
            writer.WriteEndElement();
        }

        static void WriteItemContents(XmlWriter writer, SiteMapIndexNode item)
        {
            string urlAsString = item.Url.ToString();
            writer.WriteElementString(Constants.LocationTag, urlAsString);
            if (item.LastModified.HasValue)
            {
                string formatted = item.LastModified.GetValueOrDefault().ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture);
                writer.WriteElementString(Constants.LastModifiedTag, formatted);
            }
        }

        static void WriteItems(XmlWriter writer, IEnumerable<SiteMapIndexNode> items)
        {
            foreach (SiteMapIndexNode item in items)
            {
                WriteItem(writer, item);
            }
        }
    }
}