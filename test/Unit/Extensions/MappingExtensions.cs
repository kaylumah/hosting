// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Ssg.Extensions.Metadata.Abstractions;
using Test.Unit.Entities;
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Unit.Extensions
{
    public static partial class MappingExtensions
    {
        public static File ToFile(this PageMetaData pageMetaData)
        {
            FileMetaData fileMetaData = new FileMetaData();
            Dictionary<string, object> data = pageMetaData;
            foreach (KeyValuePair<string, object> item in data)
            {
                fileMetaData.Add(item.Key, item.Value);
            }

            File file = new File(fileMetaData, string.Empty);
            return file;
        }

        public static IEnumerable<File> ToFile(this IEnumerable<PageMetaData> pageMetaData)
        {
            return pageMetaData.Select(ToFile);
        }

        public static PageMetaData ToPageMetaData(this Article article)
        {
            List<object> tags = article.Tags.Cast<object>().ToList();
            Dictionary<string, object> pageDictionary = new Dictionary<string, object>();
            pageDictionary.SetValue(nameof(PageMetaData.Uri), article.Uri);
            pageDictionary.SetValue(nameof(PageMetaData.Name), article.Uri);
            pageDictionary.SetValue(nameof(PageMetaData.Title), article.Title);
            pageDictionary.SetValue(nameof(PageMetaData.Description), article.Description);
            pageDictionary.SetValue(nameof(PageMetaData.Author), article.Author);
            pageDictionary.SetValue("PublishedDate", article.Created.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            pageDictionary.SetValue("ModifiedDate", article.Modified.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            pageDictionary.SetValue(nameof(PageMetaData.Type), "Article");
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
            PageMetaData[] pages = files.ToPages(siteGuid);
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
}
