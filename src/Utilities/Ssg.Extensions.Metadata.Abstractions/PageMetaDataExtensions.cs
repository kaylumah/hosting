// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public static class PageMetaDataExtensions
    {
        public static readonly Func<PageMetaData, bool> Tags;

        public static readonly Func<Article, bool> Series;

        public static readonly Func<PageMetaData, bool> Html;

        public static readonly Func<Article, bool> Featured;

        static PageMetaDataExtensions()
        {
            Tags = (page) => page.Tags != null && 0 < page.Tags.Count;
            Series = (page) => !string.IsNullOrEmpty(page.Series);
            Html = (page) => page.IsHtml();
            Featured = (page) => page.Featured;
        }

        public static string GetExtension(this PageMetaData pageMetaData)
        {
            string result = Path.GetExtension(pageMetaData.Uri);
            return result;
        }

        public static bool IsExtension(this PageMetaData pageMetaData, string target)
        {
            ArgumentNullException.ThrowIfNull(target);
            string actual = pageMetaData.GetExtension();
            bool result = target.Equals(actual, StringComparison.Ordinal);
            return result;
        }

        public static bool IsUrl(this PageMetaData pageMetaData, string url)
        {
            // TODO use this more
            bool result = url.Equals(pageMetaData.Uri, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsHtml(this PageMetaData pageMetaData)
        {
            bool result = pageMetaData.IsExtension(".html");
            return result;
        }

        public static IEnumerable<PageMetaData> HasTag(this IEnumerable<PageMetaData> source)
        {
            IEnumerable<PageMetaData> result = source.Where(Tags);
            return result;
        }

        public static IEnumerable<PageMetaData> FromTag(this IEnumerable<PageMetaData> source, string tag)
        {
            IEnumerable<PageMetaData> result = source
                .HasTag()
                .Where(page => page.Tags.Contains(tag));
            return result;
        }

        public static IEnumerable<Article> HasSeries(this IEnumerable<Article> source)
        {
            IEnumerable<Article> result = source.Where(Series);
            return result;
        }

        public static IEnumerable<Article> FromSeries(this IEnumerable<Article> source, string series)
        {
            IEnumerable<Article> result = source
                .HasSeries()
                .Where(page => page.Series.Equals(series, StringComparison.Ordinal));
            return result;
        }

        public static bool IsContentType(this PageMetaData page, string contentType)
        {
            bool result = page.Type != null && page.Type.Equals(contentType, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsArticle(this PageMetaData page)
        {
            bool result = page.IsContentType("Article");
            return result;
        }

        public static bool IsPage(this PageMetaData page)
        {
            bool result = page.IsContentType("Page");
            return result;
        }

        public static bool IsCollection(this PageMetaData page)
        {
            bool result = page.IsContentType("Collection");
            return result;
        }

        public static IEnumerable<PageMetaData> IsArticle(this IEnumerable<PageMetaData> source)
        {
            IEnumerable<PageMetaData> result = source.Where(IsArticle);
            return result;
        }

        public static IEnumerable<Article> IsFeatured(this IEnumerable<Article> source)
        {
            IEnumerable<Article> result = source.Where(Featured);
            return result;
        }

        public static IEnumerable<Article> ByRecentlyPublished(this IEnumerable<Article> source)
        {
            IOrderedEnumerable<Article> result = source.OrderByDescending(x => x.Published);
            return result;
        }

        public static IEnumerable<PublicationMetaData> ByRecentlyPublished(this IEnumerable<PublicationMetaData> source)
        {
            IOrderedEnumerable<PublicationMetaData> result = source.OrderByDescending(x => x.Published);
            return result;
        }

        public static SortedDictionary<string, List<PageId>> GetPagesByTag(this IEnumerable<Article> source)
        {
            SortedDictionary<string, List<PageId>> result = new(StringComparer.OrdinalIgnoreCase);
            foreach (Article article in source)
            {
                List<string> tags = article.Tags;
                foreach (string tag in tags)
                {
                    if (result.ContainsKey(tag) == false)
                    {
                        result[tag] = new();
                    }

                    result[tag].Add(article.Id);
                }
            }

            return result;
        }

        public static SortedDictionary<int, List<PageId>> GetPagesByYear(this IEnumerable<Article> source)
        {
            DescendingComparer<int> comparer = new DescendingComparer<int>();
            SortedDictionary<int, List<PageId>> result = new(comparer);
            foreach (Article article in source)
            {
                DateTimeOffset published = article.Published;
                int year = published.Year;
                if (result.ContainsKey(year) == false)
                {
                    result[year] = new();
                }

                result[year].Add(article.Id);
            }

            return result;
        }
    }
}