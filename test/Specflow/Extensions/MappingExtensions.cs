// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Test.Specflow.Entities;
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Specflow.Extensions;

public static partial class MappingExtensions
{
    public static IEnumerable<Article> ToArticles(this IEnumerable<File> files, Guid siteGuid = default)
    {
        var pages = files.ToPages(siteGuid);
        return pages.ToArticles();
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
    
    public static IEnumerable<Article> ToArticles(this IEnumerable<PageMetaData> pageMetaData)
    {
        return pageMetaData.Select(ToArticle);
    }
}
