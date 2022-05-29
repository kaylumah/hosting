// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public static class SiteMapExtensions
{
    public static byte[] SaveAsXml(this SiteMap siteMap)
    {
        var settings = new XmlWriterSettings()
        {
            Indent = true,
            Encoding = new System.Text.UTF8Encoding(false)
        };
        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);
        siteMap.SaveAsXml(writer);
        writer.Close();
        return stream.ToArray();
    }
}
