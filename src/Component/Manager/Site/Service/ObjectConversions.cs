// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class ObjectConversions
    {
        public static AuthorId AuthorId(string author)
        {
            return author;
        }

        public static IEnumerable<object> TagCloud(SiteMetaData site)
        {
            PageViewCollection pageViews = site.Tags;
            IEnumerable<object> y = pageViews.Select(pageView =>
            {
                object x = new
                {
                    Id = pageView.Id,
                    DisplayName = pageView.DisplayName,
                    Size = pageView.Size
                };
                return x;
            });
            return y;
        }

        public static IEnumerable<Article> ArticlesForTag(SiteMetaData site, string tag, int? take = null)
        {
            ArgumentNullException.ThrowIfNull(site);
            ArgumentNullException.ThrowIfNull(tag);

            bool tagExists = site.Tags.TryGetValue(tag, out PageView? resultForTag);
            IEnumerable<Article> result;
            if (tagExists && resultForTag != null)
            {
                PageMetaData[] pageMetas = resultForTag.Pages;
                IEnumerable<Article> asArticles = pageMetas.OfType<Article>();
                result = asArticles.ByRecentlyPublished();
                if (take != null)
                {
                    result = result.Take(take.Value);
                }
            }
            else
            {
                result = Enumerable.Empty<Article>();
            }

            return result;
        }
    }
}