// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public static class PageMetaDataExtensions
    {
        public static readonly Func<PublicationPageMetaData, bool> Tags;

        public static readonly Func<ArticlePublicationPageMetaData, bool> Series;

        public static readonly Func<PageMetaData, bool> Html;

        public static readonly Func<ArticlePublicationPageMetaData, bool> Featured;

        static PageMetaDataExtensions()
        {
            Tags = (page) => 0 < page.Tags.Count;
            Series = (page) => !string.IsNullOrEmpty(page.Series);
            Html = (page) => page.IsHtml();
            Featured = (page) => page.Featured;
        }

        public static PageMetaData WithLdJson(this PageMetaData page, string ldJson)
        {
            void AddLdJson(Dictionary<string, object?> data)
            {
                data[nameof(PageMetaData.LdJson)] = ldJson;
            }

            PageMetaData result = page.With(AddLdJson);
            return result;
        }

        public static PageMetaData WithMetaTags(this PageMetaData page, string metaTags)
        {
            void AddMetaTags(Dictionary<string, object?> data)
            {
                data[nameof(PageMetaData.MetaTags)] = metaTags;
            }

            PageMetaData result = page.With(AddMetaTags);
            return result;
        }

        static PageMetaData With(this PageMetaData page, Action<Dictionary<string, object?>> update)
        {
            Dictionary<string, object?> original = page;
            Dictionary<string, object?> copy = new Dictionary<string, object?>(original);
            update(copy);
            PageMetaData pageMetaData = new PageMetaData(copy);

            return pageMetaData;
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

        public static IEnumerable<PublicationPageMetaData> HasTag(this IEnumerable<PublicationPageMetaData> source)
        {
            IEnumerable<PublicationPageMetaData> result = source.Where(Tags);
            return result;
        }

        public static IEnumerable<PageMetaData> FromTag(this IEnumerable<PublicationPageMetaData> source, string tag)
        {
            IEnumerable<PageMetaData> result = source
                .HasTag()
                .Where(page => page.Tags.Contains(tag));
            return result;
        }

        public static IEnumerable<ArticlePublicationPageMetaData> HasSeries(this IEnumerable<ArticlePublicationPageMetaData> source)
        {
            IEnumerable<ArticlePublicationPageMetaData> result = source.Where(Series);
            return result;
        }

        public static IEnumerable<ArticlePublicationPageMetaData> FromSeries(this IEnumerable<ArticlePublicationPageMetaData> source, string series)
        {
            IEnumerable<ArticlePublicationPageMetaData> result = source
                .HasSeries()
                .Where(page => page.Series.Equals(series, StringComparison.Ordinal));
            return result;
        }

        public static IEnumerable<ArticlePublicationPageMetaData> IsFeatured(this IEnumerable<ArticlePublicationPageMetaData> source)
        {
            IEnumerable<ArticlePublicationPageMetaData> result = source.Where(Featured);
            return result;
        }

        public static IEnumerable<ArticlePublicationPageMetaData> ByRecentlyPublished(this IEnumerable<ArticlePublicationPageMetaData> source)
        {
            IOrderedEnumerable<ArticlePublicationPageMetaData> result = source.OrderByDescending(x => x.Published);
            return result;
        }

        public static IEnumerable<PublicationPageMetaData> ByRecentlyPublished(this IEnumerable<PublicationPageMetaData> source)
        {
            IOrderedEnumerable<PublicationPageMetaData> result = source.OrderByDescending(x => x.Published);
            return result;
        }

        public static SortedDictionary<string, List<PageId>> GetPagesByTag(this IEnumerable<PublicationPageMetaData> source)
        {
            SortedDictionary<string, List<PageId>> result = new(StringComparer.OrdinalIgnoreCase);
            foreach (PublicationPageMetaData article in source)
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

        public static SortedDictionary<int, List<PageId>> GetPagesByYear(this IEnumerable<PublicationPageMetaData> source)
        {
            DescendingComparer<int> comparer = new DescendingComparer<int>();
            SortedDictionary<int, List<PageId>> result = new(comparer);
            foreach (PublicationPageMetaData article in source)
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