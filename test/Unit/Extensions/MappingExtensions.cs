// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

namespace Test.Unit.Extensions
{
    public static partial class MappingExtensions
    {
        public static BinaryFile ToFile(this PageMetaData pageMetaData)
        {
            FileMetaData fileMetaData = new FileMetaData();
            Dictionary<string, object?> data = pageMetaData;
            foreach (KeyValuePair<string, object?> item in data)
            {
                fileMetaData.Add(item.Key, item.Value);
            }

            BinaryFile file = new TextFile(fileMetaData, [], string.Empty);
            return file;
        }

        public static IEnumerable<BinaryFile> ToFile(this IEnumerable<PageMetaData> pageMetaData)
        {
            IEnumerable<BinaryFile> result = pageMetaData.Select(ToFile);
            return result;
        }

        public static PageMetaData ToPageMetaData(this Entities.Article article)
        {
            List<object> tags = article.Tags.Cast<object>().ToList();
            Dictionary<string, object?> pageDictionary = new Dictionary<string, object?>();
            pageDictionary.SetValue(nameof(PageMetaData.Uri), article.Uri);
            pageDictionary.SetValue(nameof(PageMetaData.Name), article.Uri);
            pageDictionary.SetValue(nameof(PageMetaData.Title), article.Title);
            pageDictionary.SetValue(nameof(PageMetaData.Description), article.Description);
            pageDictionary.SetValue(nameof(PageMetaData.Author), article.Author);
#pragma warning disable
            pageDictionary.SetValue("PublishedDate", article.Created.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            pageDictionary.SetValue("ModifiedDate", article.Modified.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
#pragma warning restore
            pageDictionary.SetValue(nameof(PageMetaData.Type), "Article");
            pageDictionary.SetValue(nameof(PageMetaData.Collection), "posts");
            pageDictionary.SetValue(nameof(ArticlePublicationPageMetaData.Feed), "true");
            pageDictionary.SetValue(nameof(PageMetaData.Sitemap), "true");
            pageDictionary.SetValue(nameof(PageMetaData.Layout), "default.html");
            pageDictionary.SetValue(nameof(PublicationPageMetaData.Tags), tags);
            PageMetaData result = new PageMetaData(pageDictionary);
            return result;
        }

        public static IEnumerable<PageMetaData> ToPageMetaData(this IEnumerable<Entities.Article> article)
        {
            IEnumerable<PageMetaData> result = article.Select(ToPageMetaData);
            return result;
        }

        public static IEnumerable<Entities.Article> ToArticles(this IEnumerable<BinaryFile> files, Guid siteGuid = default)
        {
            List<PageMetaData> pageMetas = new List<PageMetaData>();
            IEnumerable<TextFile> textFiles = files.OfType<TextFile>();
            foreach (TextFile file in textFiles)
            {
                PageMetaData page = file.ToPage(siteGuid);
                pageMetas.Add(page);
            }

            IEnumerable<Entities.Article> result = pageMetas.ToArticles();
            return result;
        }

        public static IEnumerable<Entities.Article> ToArticles(this IEnumerable<PageMetaData> pageMetaData)
        {
            IEnumerable<Entities.Article> result = pageMetaData.Select(ToArticle);
            return result;
        }

        public static Entities.Article ToArticle(this PageMetaData pageMetaData)
        {
            Entities.Article result = new Entities.Article();
            result.Uri = pageMetaData.Uri;
            result.Title = pageMetaData.Title;
            result.Description = pageMetaData.Description;
            result.Author = pageMetaData.Author;
            result.Created = pageMetaData.Published != DateTimeOffset.MinValue ? pageMetaData.Published : null;
            result.Modified = pageMetaData.Modified != DateTimeOffset.MinValue ? pageMetaData.Modified : null;
            return result;
        }
    }
}
