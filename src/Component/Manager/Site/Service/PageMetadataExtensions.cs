// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class PageMetadataExtensions
    {
        public static IEnumerable<PageMetaData> WhereIsArticle(this IEnumerable<PageMetaData> source)
        {
            return source.Where(x => ContentType.Article.Equals(x.Type));
        }
    }
}
