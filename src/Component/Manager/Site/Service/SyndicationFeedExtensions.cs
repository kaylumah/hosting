// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Xml;
using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace System.ServiceModel.Syndication
{
    public static class SyndicationFeedExtensions
    {
        public static byte[] SaveAsAtom10(this SyndicationFeed syndicationFeed)
        {
            var settings = new XmlWriterSettings();
            using var stream = new MemoryStream();
            using var writer = XmlWriter.Create(stream, settings);
            syndicationFeed.SaveAsAtom10(writer);
            writer.Close();
            return stream.ToArray();
        }
    }

    public static partial class PageMetaDataExtensions
    {
        public static SyndicationItem ToSyndicationItem(this PageMetaData pageMetaData)
        {
            return new SyndicationItem { };
        }

        public static IEnumerable<SyndicationItem> ToSyndicationItems(this IEnumerable<PageMetaData> pageMetaDatas)
        {
            return pageMetaDatas.Select(ToSyndicationItem);
        }
    }
}
