﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
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

        readonly ILogger _Logger;

        public FeedGenerator(ILogger<FeedGenerator> logger)
        {
            _Logger = logger;
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
            Uri feedUri = siteMetaData.AbsoluteUri("feed.xml");
            SyndicationFeed feed = new SyndicationFeed();
            feed.Language = siteMetaData.Language;
            feed.Title = new CDataSyndicationContent(siteMetaData.Title);
            feed.Description = new CDataSyndicationContent(siteMetaData.Description);
            feed.Id = feedUri.ToString();
            feed.Copyright = new CDataSyndicationContent(copyrightClaim);
            feed.LastUpdatedTime = generatedAtBuildTime;
            feed.ImageUrl = siteMetaData.AbsoluteUri("assets/logo_alt.svg");
            feed.Generator = "Kaylumah Site Generator";

            SyndicationLink selfLink = BuildLink(feedUri, "self", "application/atom+xml");
            feed.Links.Add(selfLink);

            Uri blogUri = siteMetaData.AbsoluteUri("blog.html");
            SyndicationLink alternateLink = BuildLink(blogUri, "alternate", "text/html");
            feed.Links.Add(alternateLink);

            return feed;
        }

        SyndicationLink BuildLink(Uri absoluteUri, string relationshipType, string mediaType)
        {
            SyndicationLink result = new SyndicationLink(absoluteUri);
            result.RelationshipType = relationshipType;
            result.MediaType = mediaType;
            return result;
        }

        List<SyndicationItem> GetPosts(SiteMetaData siteMetaData)
        {
            List<PageMetaData> posts = RetrievePostPageMetaDatas(siteMetaData)
                .ToList();
            List<SyndicationItem> result = new List<SyndicationItem>();
            if (0 < posts.Count)
            {
                FeedCount(posts.Count);
                Dictionary<AuthorId, SyndicationPerson> persons = siteMetaData.ToPersons();
                Dictionary<string, SyndicationCategory> tags = siteMetaData.ToCategories();
                foreach (PageMetaData pageMetaData in posts)
                {
                    Uri pageUri = pageMetaData.CanonicalUri;
                    string pageUrl = pageUri.ToString();
                    SyndicationItem item = new SyndicationItem();
                    item.Id = pageUrl;
                    item.Title = new CDataSyndicationContent(pageMetaData.Title);
                    item.Summary = new CDataSyndicationContent(pageMetaData.Description);
                    TextSyndicationContent htmlContent = new TextSyndicationContent(pageMetaData.Content, TextSyndicationContentKind.Html);
                    item.Content = new CDataSyndicationContent(htmlContent);
                    item.PublishDate = pageMetaData.Published;
                    item.LastUpdatedTime = pageMetaData.Modified;

                    List<SyndicationCategory> itemCategories = pageMetaData
                        .Tags
                        .Where(tags.ContainsKey)
                        .Select(tag => tags[tag])
                        .ToList();
                    itemCategories.ForEach(item.Categories.Add);
                    SyndicationLink syndicationLink = new SyndicationLink(pageUri);
                    item.Links.Add(syndicationLink);
                    if (!string.IsNullOrEmpty(pageMetaData.Author) && persons.TryGetValue(pageMetaData.Author, out SyndicationPerson? person))
                    {
                        SyndicationPerson author = person;
                        item.Authors.Add(author);
                    }

                    result.Add(item);
                }
            }

            return result;
        }

        static IEnumerable<PageMetaData> RetrievePostPageMetaDatas(SiteMetaData siteMetaData)
        {
            IEnumerable<Article> articles = siteMetaData.RecentArticles;
            IEnumerable<Article> feed = articles.Where(x => x.Feed);

            return feed;
        }
    }
}
