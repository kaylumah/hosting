// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using Kaylumah.Ssg.Manager.Site.Service.Feed;

namespace System
{
    public static partial class ByteExtensions
    {
        public static Feed ToSyndicationFeed(this byte[] bytes, string fileName)
        {
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            Feed result = new Feed(fileName, feed);
            return result;
        }
    }
}
