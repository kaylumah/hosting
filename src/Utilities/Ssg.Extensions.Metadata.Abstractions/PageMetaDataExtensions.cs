// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions;

public static class PageMetaDataExtensions
{
    public static readonly Func<PageMetaData, bool> Tags = (page) => page.Tags != null && page.Tags.Any();

    public static readonly Func<PageMetaData, bool> Series = (page) => !string.IsNullOrEmpty(page.Series);

    public static readonly Func<PageMetaData, bool> Html = (page) => page.IsHtml();

    public static readonly Func<PageMetaData, bool> Featured = (page) => page.Featured;

    public static string GetExtension(this PageMetaData pageMetaData)
    {
        var result = Path.GetExtension(pageMetaData.Uri);
        return result;
    }

    public static bool IsExtension(this PageMetaData pageMetaData, string target)
    {
        ArgumentNullException.ThrowIfNull(target);
        var actual = pageMetaData.GetExtension();
        var result = target.Equals(actual, StringComparison.Ordinal);
        return result;
    }

    public static bool IsHtml(this PageMetaData pageMetaData)
    {
        var result = pageMetaData.IsExtension(".html");
        return result;
    }

    public static IEnumerable<PageMetaData> HasTag(this IEnumerable<PageMetaData> source)
    {
        var result = source.Where(Tags);
        return result;
    }

    public static IEnumerable<PageMetaData> FromTag(this IEnumerable<PageMetaData> source, string tag)
    {
        var result = source
                .HasTag()
                .Where(page => page.Tags.Contains(tag));
        return result;
    }

    public static IEnumerable<PageMetaData> HasSeries(this IEnumerable<PageMetaData> source)
    {
        var result = source.Where(Series);
        return result;
    }

    public static IEnumerable<PageMetaData> FromSeries(this IEnumerable<PageMetaData> source, string series)
    {
        var result = source
                .HasSeries()
                .Where(page => page.Series.Equals(series, StringComparison.Ordinal));
        return result;
    }

    public static IEnumerable<PageMetaData> FromContentType(this IEnumerable<PageMetaData> source, ContentType contentType)
    {
        var result = source
                .Where(page => contentType.Equals(page.Type));
        return result;
    }

    public static IEnumerable<PageMetaData> IsArticle(this IEnumerable<PageMetaData> source)
    {
        var result = source.FromContentType(ContentType.Article);
        return result;
    }

    public static IEnumerable<PageMetaData> IsFeatured(this IEnumerable<PageMetaData> source)
    {
        var result = source.Where(Featured);
        return result;
    }

    public static IEnumerable<PageMetaData> ByRecentlyPublished(this IEnumerable<PageMetaData> source)
    {
        var result = source.OrderByDescending(x => x.Published);
        return result;
    }
}
