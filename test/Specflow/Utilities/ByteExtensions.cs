// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;
using System.Xml;

namespace Test.Specflow.Utilities;

public static class ByteExtensions
{
    public static SyndicationFeed ToSyndicationFeed(this byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var xmlReader = XmlReader.Create(stream);
        var feed = SyndicationFeed.Load(xmlReader);
        return feed;
    }
}
