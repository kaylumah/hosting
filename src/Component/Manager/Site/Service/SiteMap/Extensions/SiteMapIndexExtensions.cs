// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap
{
    public static class SiteMapIndexExtensions
    {
        public static byte[] SaveAsXml(this SiteMapIndex siteMapIndex)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new System.Text.UTF8Encoding(false);
            using MemoryStream stream = new MemoryStream();
            using XmlWriter writer = XmlWriter.Create(stream, settings);
            siteMapIndex.SaveAsXml(writer);
            writer.Close();
            byte[] result = stream.ToArray();
            return result;
        }
    }
}
