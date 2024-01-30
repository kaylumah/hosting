// Copyright (c) Kaylumah, 2024. All rights reserved.
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

        public static readonly Func<PageMetaData, bool> Series;

        public static readonly Func<PageMetaData, bool> Html;

        public static readonly Func<PageMetaData, bool> Featured;

        static PageMetaDataExtensions()
        {
            Tags = (page) => page.Tags != null && page.Tags.Count > 0;
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

        public static IEnumerable<PageMetaData> HasSeries(this IEnumerable<PageMetaData> source)
        {
            IEnumerable<PageMetaData> result = source.Where(Series);
            return result;
        }

        public static IEnumerable<PageMetaData> FromSeries(this IEnumerable<PageMetaData> source, string series)
        {
            IEnumerable<PageMetaData> result = source
                    .HasSeries()
                    .Where(page => page.Series.Equals(series, StringComparison.Ordinal));
            return result;
        }

        public static IEnumerable<PageMetaData> FromContentType(this IEnumerable<PageMetaData> source, ContentType contentType)
        {
            IEnumerable<PageMetaData> result = source
                    .Where(page => contentType.Equals(page.Type));
            return result;
        }

        public static IEnumerable<PageMetaData> IsArticle(this IEnumerable<PageMetaData> source)
        {
            IEnumerable<PageMetaData> result = source.FromContentType(ContentType.Article);
            return result;
        }

        public static IEnumerable<PageMetaData> IsFeatured(this IEnumerable<PageMetaData> source)
        {
            IEnumerable<PageMetaData> result = source.Where(Featured);
            return result;
        }

        public static IEnumerable<PageMetaData> ByRecentlyPublished(this IEnumerable<PageMetaData> source)
        {
            IOrderedEnumerable<PageMetaData> result = source.OrderByDescending(x => x.Published);
            return result;
        }
    }
}
