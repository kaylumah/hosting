// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Search;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface ISiteArtifactPlugin
    {
        Artifact[] Generate(SiteMetaData siteMetaData);
    }

    public class SearchIndexArtifactPlugin : ISiteArtifactPlugin
    {
        public Artifact[] Generate(SiteMetaData siteMetaData)
        {
            IEnumerable<Article> articles = siteMetaData.GetArticles();
            List<IndexItem> indexItems = new List<IndexItem>();
            foreach (Article article in articles)
            {
                IndexItem indexItem = new IndexItem(article.Id, article.Title);
                indexItems.Add(indexItem);
            }

            IndexItem[] array = indexItems.ToArray();
            SearchIndex searchIndex = new SearchIndex(array);
            byte[] bytes = searchIndex.SaveAsJson();
            Artifact artifact = new Artifact("search.json", bytes);
            Artifact[] result = [artifact];
            return result;
        }
    }

    public class SiteMapSiteArtifactPlugin : ISiteArtifactPlugin
    {
        readonly SiteMapGenerator _SiteMapGenerator;

        public SiteMapSiteArtifactPlugin(SiteMapGenerator siteMapGenerator)
        {
            _SiteMapGenerator = siteMapGenerator;
        }

        public Artifact[] Generate(SiteMetaData siteMetaData)
        {
            SiteMap.SiteMap sitemap = _SiteMapGenerator.Create(siteMetaData);
            byte[] bytes = sitemap
                    .SaveAsXml();
            Artifact siteMapAsArtifact = new Artifact("sitemap.xml", bytes);
            Artifact[] result = [siteMapAsArtifact];
            return result;
        }
    }

    public class FeedSiteArtifactPlugin : ISiteArtifactPlugin
    {
        readonly FeedGenerator _FeedGenerator;

        public FeedSiteArtifactPlugin(FeedGenerator feedGenerator)
        {
            _FeedGenerator = feedGenerator;
        }

        public Artifact[] Generate(SiteMetaData siteMetaData)
        {
            System.ServiceModel.Syndication.SyndicationFeed feed = _FeedGenerator.Create(siteMetaData);
            byte[] bytes = feed
                    .SaveAsAtom10();
            Artifact feedAsArtifact = new Artifact("feed.xml", bytes);
            Artifact[] result = [feedAsArtifact];
            return result;
        }
    }
}
