// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class PageMetadataExtensions
    {

        public static IEnumerable<PageMetaData> WhereIsSeries(this IEnumerable<PageMetaData> source)
        {
            return source
                .Where(x => !string.IsNullOrEmpty(x.Series));
        }

        public static IEnumerable<PageMetaData> WhereSeriesIs(this IEnumerable<PageMetaData> source, string series)
        {
            return source
                .WhereIsSeries()
                .Where(page => page.Equals(series));
        }

        public static IEnumerable<PageMetaData> WhereIsTagged(this IEnumerable<PageMetaData> source)
        {
            return source
                .Where(x => x.Tags != null)
                .Where(x => x.Tags.Any());
        }

        public static IEnumerable<PageMetaData> WhereIsTaggedWith(this IEnumerable<PageMetaData> source, string tag)
        {
            return source
                .WhereIsTagged()
                .Where(page => page.Tags.Contains(tag));
        }

        public static IEnumerable<PageMetaData> WhereIsArticle(this IEnumerable<PageMetaData> source)
        {
            return source.Where(x => ContentType.Article.Equals(x.Type));
        }
    }
}
