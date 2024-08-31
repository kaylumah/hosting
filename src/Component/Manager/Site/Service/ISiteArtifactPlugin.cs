﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface ISiteArtifactPlugin
    {
        Artifact[] Generate(SiteMetaData siteMetaData);
    }

    public class SiteMapSiteArtifactPlugin : ISiteArtifactPlugin
    {
        Artifact[] ISiteArtifactPlugin.Generate(SiteMetaData siteMetaData)
        {
            List<SiteMap.SiteMap> siteMaps = new List<SiteMap.SiteMap>();
            // TODO: Consider the split
            SiteMap.SiteMap generatedSiteMap = CreateDefault(siteMetaData);
            siteMaps.Add(generatedSiteMap);

            List<SiteMapIndexNode> siteMapIndexNodes = new List<SiteMapIndexNode>();
            List<Artifact> siteMapArtifacts = new List<Artifact>();
            foreach (SiteMap.SiteMap siteMap in siteMaps)
            {
                SiteMapIndexNode siteMapIndexNode = new SiteMapIndexNode();
                siteMapIndexNode.Url = siteMetaData.AbsoluteUri(siteMap.FileName);
                siteMapIndexNode.LastModified = siteMap.LastModified;
                siteMapIndexNodes.Add(siteMapIndexNode);

                byte[] bytes = siteMap.SaveAsXml();
                Artifact siteMapAsArtifact = new Artifact(siteMap.FileName, bytes);
                siteMapArtifacts.Add(siteMapAsArtifact);
            }

            SiteMapIndex siteMapIndex = new SiteMapIndex("sitemap_index.xml", siteMapIndexNodes);
            byte[] siteMapIndexBytes = siteMapIndex.SaveAsXml();
            Artifact siteMapIndexAsArtifact = new Artifact(siteMapIndex.FileName, siteMapIndexBytes);

            List<Artifact> artifacts = new List<Artifact>();
            artifacts.AddRange(siteMapArtifacts);
            artifacts.Add(siteMapIndexAsArtifact);

            Artifact[] result = artifacts.ToArray();
            return result;
        }

        static bool IsNot404(PageMetaData pageMetaData)
        {
            bool is404 = pageMetaData.IsUrl("404.html");
            bool result = is404 == false;
            return result;
        }

        static SiteMapNode ToSiteMapNode(PageMetaData pageMetaData)
        {
            SiteMapNode node = new SiteMapNode();
            Uri siteMapUri = pageMetaData.CanonicalUri;
            node.Url = siteMapUri.ToString();
            node.LastModified = pageMetaData.Modified;
            return node;
        }

        static List<SiteMapNode> ToSiteMapNodes(IEnumerable<PageMetaData> pages)
        {
            List<SiteMapNode> siteMapNodes = new List<SiteMapNode>();
            foreach (PageMetaData page in pages)
            {
                SiteMapNode siteMapNode = ToSiteMapNode(page);
                siteMapNodes.Add(siteMapNode);
            }

            return siteMapNodes;
        }

        static SiteMap.SiteMap CreateDefault(SiteMetaData siteMetaData)
        {
            IEnumerable<PageMetaData> sitePages = siteMetaData.GetPages();
            IEnumerable<PageMetaData> htmlPages = sitePages.Where(PageMetaDataFilters.IsHtml);
            IEnumerable<PageMetaData> without404 = htmlPages.Where(IsNot404);

            List<PageMetaData> pages = without404.ToList();
            List<SiteMapNode> siteMapNodes = ToSiteMapNodes(pages);

            SiteMap.SiteMap siteMap = new SiteMap.SiteMap("sitemap.xml", siteMapNodes);
            return siteMap;
        }
    }

    public class FeedSiteArtifactPlugin : ISiteArtifactPlugin
    {
        Artifact[] ISiteArtifactPlugin.Generate(SiteMetaData siteMetaData)
        {
            Feed.FeedArtifact generatedFeed = CreateDefault(siteMetaData);
            List<Feed.FeedArtifact> feeds = new List<Feed.FeedArtifact>()
            {
                generatedFeed
            };

            List<Artifact> artifacts = new List<Artifact>();
            foreach(Feed.FeedArtifact feed in feeds)
            {
                byte[] bytes = feed.SaveAsAtom10();
                Artifact feedAsArtifact = new Artifact(feed.FileName, bytes);
                artifacts.Add(feedAsArtifact);
            }

            Artifact[] result = artifacts.ToArray();
            return result;
        }

        public Feed.FeedArtifact CreateDefault(SiteMetaData siteMetaData)
        {
            SyndicationFeed feed = GetBlogInformation(siteMetaData);
            List<SyndicationItem> posts = GetPosts(siteMetaData);
            feed.Items = posts;

            Feed.FeedArtifact result = new Feed.FeedArtifact("feed.xml", feed);
            return result;
        }

        SyndicationFeed GetBlogInformation(SiteMetaData siteMetaData)
        {
            BuildData build = siteMetaData.Build;
            string generatorVersion = build.ShortGitHash;
            string copyrightClaim = build.Copyright;
            DateTimeOffset generatedAtBuildTime = build.Time;
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
