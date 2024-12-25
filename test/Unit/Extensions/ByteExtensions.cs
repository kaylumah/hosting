// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;

namespace Test.Unit.Utilities
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
    }
}
