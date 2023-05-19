// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Test.Specflow.Entities;

namespace Test.Specflow.Utilities;

public static class SiteMapExtensions
{
    public static Article ToArticle(this SiteMapNode siteMapNode)
    {
        var article = new Article { Uri = siteMapNode.Url };
        return article;
    }

    public static IEnumerable<Article> ToArticles(this IEnumerable<SiteMapNode> siteMapNodes)
    {
        return siteMapNodes.Select(ToArticle);
    }

    public static ArticleCollection ToArticles(this SiteMap siteMap)
    {
        var articles = new ArticleCollection();
        articles.AddRange(siteMap.Items.ToArticles());
        return articles;
    }
}

public static class SyndicationFeedExtensions
{
    public static Article ToArticle(this SyndicationItem syndicationItem)
    {
        var article = new Article();
        article.Uri = syndicationItem.Id;
        article.Created = syndicationItem.PublishDate;
        article.Modified = syndicationItem.LastUpdatedTime;
        return article;
    }

    public static IEnumerable<Article> ToArticles(this IEnumerable<SyndicationItem> syndicationItems)
    {
        return syndicationItems.Select(ToArticle);
    }

    public static ArticleCollection ToArticles(this SyndicationFeed syndicationFeed)
    {
        var articles = new ArticleCollection();
        articles.AddRange(syndicationFeed.Items.ToArticles());
        return articles;
    }
}
