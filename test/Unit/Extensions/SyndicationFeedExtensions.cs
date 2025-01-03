// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Test.Unit.Entities;

namespace Test.Unit.Utilities
{
    public static class SiteMapExtensions
    {
        public static Article ToArticle(this SiteMapNode siteMapNode)
        {
            Article article = new Article();
            article.Uri = siteMapNode.Url;
            return article;
        }

        public static IEnumerable<Article> ToArticles(this IEnumerable<SiteMapNode> siteMapNodes)
        {
            IEnumerable<Article> result = siteMapNodes.Select(ToArticle);
            return result;
        }

        public static ArticleCollection ToArticles(this SiteMap siteMap)
        {
            ArticleCollection articles = new ArticleCollection();
            IEnumerable<Article> items = siteMap.Items.ToArticles();
            articles.AddRange(items);
            return articles;
        }
    }

    public static class SyndicationFeedExtensions
    {
        public static Article ToArticle(this SyndicationItem syndicationItem)
        {
            Article article = new Article();
            article.Uri = syndicationItem.Id;
            article.Created = syndicationItem.PublishDate;
            article.Modified = syndicationItem.LastUpdatedTime;
            return article;
        }

        public static IEnumerable<Article> ToArticles(this IEnumerable<SyndicationItem> syndicationItems)
        {
            IEnumerable<Article> result = syndicationItems.Select(ToArticle);
            return result;
        }

        public static ArticleCollection ToArticles(this SyndicationFeed syndicationFeed)
        {
            ArticleCollection articles = new ArticleCollection();
            IEnumerable<Article> items = syndicationFeed.Items.ToArticles();
            articles.AddRange(items);
            return articles;
        }
    }
}
