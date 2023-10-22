// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public static class SyndicationFeedExtensions
    {
        public static byte[] SaveAsAtom10(this SyndicationFeed syndicationFeed)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = new System.Text.UTF8Encoding(false)
            };
            using MemoryStream stream = new MemoryStream();
            using XmlWriter writer = XmlWriter.Create(stream, settings);
            syndicationFeed.SaveAsAtom10(writer);
            writer.Close();
            return stream.ToArray();
        }
    }
}
