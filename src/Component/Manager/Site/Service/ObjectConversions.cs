// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class ObjectConversions
    {
        public static AuthorId AuthorId(string author)
        {
            return author;
        }

        public static string PublishedTimeAgo(SiteMetaData siteMetaData, Article pageMetaData)
        {
            Debug.Assert(pageMetaData != null);
            Debug.Assert(siteMetaData != null);
            Debug.Assert(siteMetaData.Build != null);
            BuildData buildData = siteMetaData.Build;
            DateTimeOffset buildTime = buildData.Time;
            DateTimeOffset publishedTime = pageMetaData.Published;
            string result = buildTime.ToReadableRelativeTime(publishedTime);
            return result;
        }

        public static string ReadingTime(Article pageMetaData)
        {
            TimeSpan duration = pageMetaData.Duration;
            string result = duration.ToReadableDuration();
            return result;
        }

        public static IEnumerable<TagViewModel> TagCloud(SiteMetaData site)
        {
            SortedDictionary<string, PageMetaData[]> tags = site.Tags;
            TagMetaDataCollection tagMetaData = site.TagMetaData;
            List<TagViewModel> result = new List<TagViewModel>();
            foreach (KeyValuePair<string, PageMetaData[]> item in tags)
            {
                string tag = item.Key;
                string displayName = item.Key;
                string description = string.Empty;
                PageMetaData[] items = item.Value;
                bool success = tagMetaData.TryGetValue(tag, out TagMetaData? tagData);
                if (success && tagData != null)
                {
                    displayName = tagData.Name;
                    description = tagData.Description;
                }

                TagViewModel resultForTag = new TagViewModel(tag, displayName, description, items.Length);
                result.Add(resultForTag);
            }

            return result;
        }

        public static TagViewModel GetTag(SiteMetaData site, string tag)
        {
            TagViewModel tagViewModel = site.GetTagViewModel(tag);
            return tagViewModel;
        }

        public static Uri AbsoluteUri(SiteMetaData site, string relativeUrl)
        {
            Uri result = site.AbsoluteUri(relativeUrl);
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

        public static string ToJson(object o)
        {
#pragma warning disable CA1869
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string result = JsonSerializer.Serialize(o, options);
            return result;
        }

        public static string DateToPattern(DateTimeOffset date, string pattern)
        {
            string result = date.ToString(pattern, CultureInfo.InvariantCulture);
            return result;
        }

        public static string DateToXmlschema(DateTimeOffset date)
        {
            string result = DateToPattern(date, "o");
            return result;
        }
    }
}