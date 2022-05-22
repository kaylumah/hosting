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

            var build = siteMetaData.Build;
            var generatorVersion = build.ShortGitHash;
            var copyrightClaim = build.Copyright;
            var generatedAtBuildTime = build.Time;

            var feed = new SyndicationFeed();
            feed.Language = siteMetaData.Language;
            feed.Title = new TextSyndicationContent(siteMetaData.Title);
            feed.Description = new TextSyndicationContent(siteMetaData.Description);
            feed.Id = GlobalFunctions.AbsoluteUrl("feed.xml");
            feed.Copyright = new TextSyndicationContent(copyrightClaim);
            feed.LastUpdatedTime = generatedAtBuildTime;
            feed.ImageUrl = new Uri(GlobalFunctions.AbsoluteUrl("assets/logo_alt.svg"));
            feed.Generator = "Kaylumah Site Generator";

            /*
            SyndicationLink link = new SyndicationLink(new Uri("http://server/link"), "alternate", "Link Title", "text/html", 1000);
            feed.Links.Add(link);
            */

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
