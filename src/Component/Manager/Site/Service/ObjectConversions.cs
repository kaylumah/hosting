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
            SortedDictionary<string, PageMetaData[]> tags = site.Tags;
            TagMetaDataCollection tagMetaData = site.TagMetaData;
            List<object> result = new List<object>();
            foreach (KeyValuePair<string, PageMetaData[]> item in tags)
            {
                string tag = item.Key;
                string displayName = item.Key;
                PageMetaData[] items = item.Value;
                bool success = tagMetaData.TryGetValue(tag, out TagMetaData? tagData);
                if (success && tagData != null)
                {
                    displayName = tagData.Name;
                }

                object resultForTag = new
                {
                    Id = tag,
                    DisplayName = displayName,
                    Size = items.Length
                };
                result.Add(resultForTag);
            }

            return result;
        }

        public static IEnumerable<Article> ArticlesForTag(SiteMetaData site, string tag, int? take = null)
        {
            ArgumentNullException.ThrowIfNull(site);
            ArgumentNullException.ThrowIfNull(tag);

            bool tagExists = site.Tags.TryGetValue(tag, out PageMetaData[]? resultForTag);
            IEnumerable<Article> result;
            if (tagExists && resultForTag != null)
            {
                IEnumerable<Article> asArticles = resultForTag.OfType<Article>();
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