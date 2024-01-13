// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

namespace System
{
    public static partial class ByteExtensions
    {
        public static SyndicationFeed ToSyndicationFeed(this byte[] bytes)
        {
            using MemoryStream stream = new MemoryStream(bytes);
            using XmlReader xmlReader = XmlReader.Create(stream);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            return feed;
        }
    }
}
