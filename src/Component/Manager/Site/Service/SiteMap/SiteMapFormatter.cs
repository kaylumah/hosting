// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap
{
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
                string formatted = item.LastModified.GetValueOrDefault().ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
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
