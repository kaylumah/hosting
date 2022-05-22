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
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };
            using var stream = new MemoryStream();
            using var writer = XmlWriter.Create(stream, settings);
            syndicationFeed.SaveAsAtom10(writer);
            writer.Close();
            return stream.ToArray();
        }
    }

    public static partial class SiteMetaDataExtensions
    {
        public static SyndicationFeed ToSyndicationFeed(this SiteMetaData siteMetaData)
        {
            // See:
            // https://docs.microsoft.com/en-us/dotnet/api/system.servicemodel.syndication.syndicationfeed?view=dotnet-plat-ext-6.0

            var feed = new SyndicationFeed("Feed Title", "Feed Description", new Uri("http://Feed/Alternate/Link"), "FeedID", DateTime.Now);

            if (siteMetaData.Collections.TryGetValue("posts", out var posts))
            {
                var feedPosts = posts
                    .OrderByDescending(x => x["date"])
                    .Where(x => bool.Parse((string)x["feed"]))
                    .ToList();
                feed.Items = posts.ToSyndicationItems();
            }

            return feed;
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
