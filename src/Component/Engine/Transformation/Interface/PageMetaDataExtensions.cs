// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

public static class PageMetaDataExtensions
{
    public static readonly Func<PageMetaData, bool> Series = (page) => !string.IsNullOrEmpty(page.Series);

    public static readonly Func<PageMetaData, bool> Html = (page) => page.IsHtml();

    public static string GetExtension(this PageMetaData pageMetaData)
    {
        return Path.GetExtension(pageMetaData.Url);
    }

    public static bool IsExtension(this PageMetaData pageMetaData, string target)
    {
        ArgumentNullException.ThrowIfNull(target);
        var actual = pageMetaData.GetExtension();
        return target.Equals(actual, StringComparison.Ordinal);
    }

    public static bool IsHtml(this PageMetaData pageMetaData)
    {
        return pageMetaData.IsExtension(".html");
    }

    public static IEnumerable<PageMetaData> HasSeries(this IEnumerable<PageMetaData> source)
    {
        return source.Where(Series);
    }

    public static IEnumerable<PageMetaData> FromSeries(this IEnumerable<PageMetaData> source, string series)
    {
        return source
                .HasSeries()
                .Where(page => page.Series.Equals(series, StringComparison.Ordinal));
    }
}
