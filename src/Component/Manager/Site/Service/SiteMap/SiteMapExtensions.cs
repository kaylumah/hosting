// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap
{
    public static class SiteMapExtensions
    {
        public static byte[] SaveAsXml(this SiteMap siteMap)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = new System.Text.UTF8Encoding(false)
            };
            using MemoryStream stream = new MemoryStream();
            using XmlWriter writer = XmlWriter.Create(stream, settings);
            siteMap.SaveAsXml(writer);
            writer.Close();
            return stream.ToArray();
        }
    }
}
