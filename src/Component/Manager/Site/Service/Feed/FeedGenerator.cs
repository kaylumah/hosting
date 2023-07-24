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

        private readonly ILogger _logger;

        public FeedGenerator(ILogger<FeedGenerator> logger)
        {
            _logger = logger;
        }

        public SyndicationFeed Create(SiteMetaData siteMetaData)
        {
            var feed = GetBlogInformation(siteMetaData);
            var posts = GetPosts(siteMetaData);
            feed.Items = posts;
            return feed;
        }

        private SyndicationFeed GetBlogInformation(SiteMetaData siteMetaData)
        {
            var build = siteMetaData.Build;
            var generatorVersion = build.ShortGitHash;
            var copyrightClaim = build.Copyright;
            var generatedAtBuildTime = build.Time;
            LogCreateBlog(generatorVersion);

            var feed = new SyndicationFeed
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



        private List<SyndicationItem> GetPosts(SiteMetaData siteMetaData)
        {
            var posts = RetrievePostPageMetaDatas(siteMetaData)
                .ByRecentlyPublished()
                .ToList();
            var result = new List<SyndicationItem>();
            if (posts.Any())
            {
                FeedCount(posts.Count);
                var persons = siteMetaData.ToPersons();
                var tags = siteMetaData.ToCategories();
                foreach (var pageMetaData in posts)
                {
                    var pageUrl = GlobalFunctions.AbsoluteUrl(pageMetaData.Uri);
                    var item = new SyndicationItem
                    {
                        Id = pageUrl,
                        Title = new CDataSyndicationContent(pageMetaData.Title),
                        Summary = new CDataSyndicationContent(pageMetaData.Description),
                        Content = new CDataSyndicationContent(new TextSyndicationContent(pageMetaData.Content, TextSyndicationContentKind.Html)),
                        PublishDate = pageMetaData.Published,
                        LastUpdatedTime = pageMetaData.Modified
                    };

                    var itemCategories = pageMetaData
                        .Tags
                        .Where(tag => tags.ContainsKey(tag))
                        .Select(tag => tags[tag])
                        .ToList();
                    itemCategories.ForEach(category => item.Categories.Add(category));
                    item.Links.Add(new SyndicationLink(new Uri(pageUrl)));
                    if (!string.IsNullOrEmpty(pageMetaData.Author) && persons.TryGetValue(pageMetaData.Author, out SyndicationPerson person))
                    {
                        var author = person;
                        item.Authors.Add(author);
                    }
                    result.Add(item);
                }
            }
            return result
                .ToList();
        }

        private static IEnumerable<PageMetaData> RetrievePostPageMetaDatas(SiteMetaData siteMetaData)
        {
            if (siteMetaData.Collections.TryGetValue("posts", out var posts))
            {
                return posts
                    .Where(x => x.Feed)
                    .ToList();
            }
            return Enumerable.Empty<PageMetaData>();
        }
    }
}
