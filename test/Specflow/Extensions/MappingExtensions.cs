// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Test.Specflow.Entities;
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Specflow.Extensions;

public static partial class MappingExtensions
{
    public static File ToFile(this PageMetaData pageMetaData)
    {
        var fileMetaData = new FileMetaData();
        foreach (var item in pageMetaData)
        {
            fileMetaData.Add(item.Key, item.Value);
        }

        var file = new File()
        {
            Name = (string)fileMetaData.GetValueOrDefault("uri", string.Empty),
            Content = string.Empty,
            LastModified = default,
            MetaData = fileMetaData
        };
        return file;
    }

    public static IEnumerable<File> ToFile(this IEnumerable<PageMetaData> pageMetaData)
    {
        return pageMetaData.Select(ToFile);
    }

    public static PageMetaData ToPageMetaData(this Article article)
    {
        var tags = article.Tags.Cast<object>().ToList();
        var pageDictionary = new Dictionary<string, object>();
        pageDictionary.SetValue(nameof(PageMetaData.Uri), article.Uri);
        pageDictionary.SetValue(nameof(PageMetaData.Name), article.Uri);
        pageDictionary.SetValue(nameof(PageMetaData.Title), article.Title);
        pageDictionary.SetValue(nameof(PageMetaData.Description), article.Description);
        pageDictionary.SetValue(nameof(PageMetaData.Author), article.Author);
        pageDictionary.SetValue(nameof(PageMetaData.Published), article.Created);
        pageDictionary.SetValue(nameof(PageMetaData.Modified), article.Modified);
        pageDictionary.SetValue(nameof(PageMetaData.Type), ContentType.Article.ToString());
        pageDictionary.SetValue(nameof(PageMetaData.Collection), "posts");
        pageDictionary.SetValue(nameof(PageMetaData.Feed), "true");
        pageDictionary.SetValue(nameof(PageMetaData.Sitemap), "true");
        pageDictionary.SetValue(nameof(PageMetaData.Layout), "default.html");
        pageDictionary.SetValue(nameof(PageMetaData.Tags), tags);
        return new PageMetaData(pageDictionary);
    }

    public static IEnumerable<PageMetaData> ToPageMetaData(this IEnumerable<Article> article)
    {
        return article.Select(ToPageMetaData);
    }

    public static IEnumerable<Article> ToArticles(this IEnumerable<File> files, Guid siteGuid = default)
    {
        var pages = files.ToPages(siteGuid);
        return pages.ToArticles();
    }

    public static IEnumerable<Article> ToArticles(this IEnumerable<PageMetaData> pageMetaData)
    {
        return pageMetaData.Select(ToArticle);
    }

    public static Article ToArticle(this PageMetaData pageMetaData)
    {
        return new Article()
        {
            Uri = pageMetaData.Uri,
            Title = pageMetaData.Title,
            Description = pageMetaData.Description,
            Author = pageMetaData.Author,
            Created = pageMetaData.Published != DateTimeOffset.MinValue ? pageMetaData.Published : null,
            Modified = pageMetaData.Modified != DateTimeOffset.MinValue ? pageMetaData.Modified : null
        };
    }
}
