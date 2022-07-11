// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;
using Test.Specflow.Entities;

namespace Test.Specflow.Utilities;

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
