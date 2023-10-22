// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public partial class FeedGenerator
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Begin BlogInformation `{Version}`")]
        public partial void LogCreateBlog(string version);

        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Information,
            Message = "Feed will have `{PostCount}` posts")]
        public partial void FeedCount(int postCount);

        readonly ILogger _logger;

        public FeedGenerator(ILogger<FeedGenerator> logger)
        {
            _logger = logger;
        }

        public SyndicationFeed Create(SiteMetaData siteMetaData)
        {
            SyndicationFeed feed = GetBlogInformation(siteMetaData);
            List<SyndicationItem> posts = GetPosts(siteMetaData);
            feed.Items = posts;
            return feed;
        }

        SyndicationFeed GetBlogInformation(SiteMetaData siteMetaData)
        {
            BuildData build = siteMetaData.Build;
            string generatorVersion = build.ShortGitHash;
            string copyrightClaim = build.Copyright;
            DateTimeOffset generatedAtBuildTime = build.Time;
            LogCreateBlog(generatorVersion);

            SyndicationFeed feed = new SyndicationFeed
            {
                Language = siteMetaData.Language,
                Title = new CDataSyndicationContent(siteMetaData.Title),
                Description = new CDataSyndicationContent(siteMetaData.Description),
                Id = GlobalFunctions.AbsoluteUrl("feed.xml"),
                Copyright = new CDataSyndicationContent(copyrightClaim),
                LastUpdatedTime = generatedAtBuildTime,
                ImageUrl = new Uri(GlobalFunctions.AbsoluteUrl("assets/logo_alt.svg")),
                Generator = "Kaylumah Site Generator"
            };

            feed.Links.Add(new SyndicationLink(new Uri(GlobalFunctions.AbsoluteUrl("feed.xml")))
            {
                RelationshipType = "self",
                MediaType = "application/atom+xml",
            });

            feed.Links.Add(new SyndicationLink(new Uri(GlobalFunctions.AbsoluteUrl("blog.html")))
            {
                RelationshipType = "alternate",
                MediaType = "text/html",
            });

            feed.Links.Add(new SyndicationLink(new Uri(GlobalFunctions.AbsoluteUrl("archive.html")))
            {
                RelationshipType = "related",
                MediaType = "text/html",
            });
            return feed;
        }

        List<SyndicationItem> GetPosts(SiteMetaData siteMetaData)
        {
            List<PageMetaData> posts = RetrievePostPageMetaDatas(siteMetaData)
                .ByRecentlyPublished()
                .ToList();
            List<SyndicationItem> result = new List<SyndicationItem>();
            if (posts.Any())
            {
                FeedCount(posts.Count);
                Dictionary<string, SyndicationPerson> persons = siteMetaData.ToPersons();
                Dictionary<string, SyndicationCategory> tags = siteMetaData.ToCategories();
                foreach (PageMetaData pageMetaData in posts)
                {
                    string pageUrl = GlobalFunctions.AbsoluteUrl(pageMetaData.Uri);
                    SyndicationItem item = new SyndicationItem
                    {
                        Id = pageUrl,
                        Title = new CDataSyndicationContent(pageMetaData.Title),
                        Summary = new CDataSyndicationContent(pageMetaData.Description),
                        Content = new CDataSyndicationContent(new TextSyndicationContent(pageMetaData.Content, TextSyndicationContentKind.Html)),
                        PublishDate = pageMetaData.Published,
                        LastUpdatedTime = pageMetaData.Modified
                    };

                    List<SyndicationCategory> itemCategories = pageMetaData
                        .Tags
                        .Where(tags.ContainsKey)
                        .Select(tag => tags[tag])
                        .ToList();
                    itemCategories.ForEach(item.Categories.Add);
                    item.Links.Add(new SyndicationLink(new Uri(pageUrl)));
                    if (!string.IsNullOrEmpty(pageMetaData.Author) && persons.TryGetValue(pageMetaData.Author, out SyndicationPerson person))
                    {
                        SyndicationPerson author = person;
                        item.Authors.Add(author);
                    }

                    result.Add(item);
                }
            }

            return result
                .ToList();
        }

        static IEnumerable<PageMetaData> RetrievePostPageMetaDatas(SiteMetaData siteMetaData)
        {
            if (siteMetaData.Collections.TryGetValue("posts", out PageMetaData[] posts))
            {
                return posts
                    .Where(x => x.Feed)
                    .ToList();
            }

            return Enumerable.Empty<PageMetaData>();
        }
    }
}
