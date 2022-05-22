// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Xml;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;

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
            // https://github.com/kestrelblackmore/BlogMatrix/blob/master/App_Code/RssSyndicator.cs
            // https://github.com/kestrelblackmore/BlogMatrix/blob/master/feed.cshtml

            SyndicationFeed feed = new SyndicationFeed("Feed Title", "Feed Description", new Uri("http://Feed/Alternate/Link"), "FeedID", DateTime.Now);

            SyndicationPerson sp = new SyndicationPerson("jesper@contoso.com", "Jesper Aaberg", "http://Jesper/Aaberg");
            feed.Authors.Add(sp);

            SyndicationCategory category = new SyndicationCategory("FeedCategory", "CategoryScheme", "CategoryLabel");
            feed.Categories.Add(category);

            feed.Contributors.Add(new SyndicationPerson("lene@contoso.com", "Lene Aaling", "http://lene/aaling"));
            feed.Copyright = new TextSyndicationContent("Copyright 2007");
            feed.Description = new TextSyndicationContent("This is a sample feed");

            feed.Generator = "Sample Code";
            feed.Id = "FeedID";
            feed.ImageUrl = new Uri("http://server/image.jpg");

            TextSyndicationContent textContent = new TextSyndicationContent("Some text content");
            SyndicationItem item = new SyndicationItem("Item Title", textContent, new Uri("http://server/items"), "ItemID", DateTime.Now);

            List<SyndicationItem> items = new List<SyndicationItem>();
            items.Add(item);
            feed.Items = items;

            feed.Language = "en-us";
            feed.LastUpdatedTime = DateTime.Now;

            SyndicationLink link = new SyndicationLink(new Uri("http://server/link"), "alternate", "Link Title", "text/html", 1000);
            feed.Links.Add(link);

            /*
            var feed = new SyndicationFeed();
            feed.Language = siteMetaData.Language;
            feed.Id = siteMetaData.Id;
            feed.Title = new TextSyndicationContent(siteMetaData.Title);
            */

            /*
            if (siteMetaData.Collections.TryGetValue("posts", out var posts))
            {
                var feedPosts = posts
                    .OrderByDescending(x => x["date"])
                    .Where(x => bool.Parse((string)x["feed"]))
                    .ToList();
                feed.Items = posts.ToSyndicationItems();
            }

            feed.Items = feed.Items.OrderByDescending(x => x.PublishDate);
            */
            return feed;
        }
    }

    public static partial class PageMetaDataExtensions
    {
        public static SyndicationItem ToSyndicationItem(this PageMetaData pageMetaData)
        {
            var pageUrl = GlobalFunctions.AbsoluteUrl(pageMetaData.Url);

            var item = new SyndicationItem
            {
                Id = pageUrl,
                Title = new TextSyndicationContent(pageMetaData.Title),
                Summary = new TextSyndicationContent(pageMetaData.Description),
                Content = new TextSyndicationContent(pageMetaData.Content, TextSyndicationContentKind.Html),
                PublishDate = (DateTimeOffset)pageMetaData["date"],
                LastUpdatedTime = pageMetaData.LastModified
            };

            item.Links.Add(new SyndicationLink(new Uri(pageUrl)));
            return item;
        }

        public static IEnumerable<SyndicationItem> ToSyndicationItems(this IEnumerable<PageMetaData> pageMetaDatas)
        {
            return pageMetaDatas.Select(ToSyndicationItem);
        }
    }
}
