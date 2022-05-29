// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public class SiteMapFormatter
{
    private readonly SiteMap _siteMap;

    public SiteMapFormatter(SiteMap siteMap)
    {
        _siteMap = siteMap;
    }

    public void WriteXml(XmlWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WriteStartElement(SiteMapConstants.UrlSetTag, SiteMapConstants.SiteMapNamespace);
        WriteItems(writer, _siteMap.Items);
    }

    private static void WriteItem(XmlWriter writer, SiteMapNode item)
    {
        writer.WriteStartElement(SiteMapConstants.UrlTag);
        WriteItemContents(writer, item);
        writer.WriteEndElement();
    }

    private static void WriteItemContents(XmlWriter writer, SiteMapNode item)
    {
        writer.WriteElementString(SiteMapConstants.LocationTag, item.Url);
        if (item.LastModified.HasValue)
        {
            writer.WriteElementString(SiteMapConstants.LastModifiedTag, item.LastModified.GetValueOrDefault().ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture));
        }
    }

    private static void WriteItems(XmlWriter writer, IEnumerable<SiteMapNode> items)
    {
        foreach(var item in items)
        {
            WriteItem(writer, item);
        }
    }
}
