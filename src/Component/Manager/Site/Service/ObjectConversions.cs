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

        public static string PublishedTimeAgo(SiteMetaData siteMetaData, PageMetaData pageMetaData)
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

        public static string ReadingTime(PageMetaData pageMetaData)
        {
            string content = pageMetaData.Content;
            TimeSpan duration = content.Duration();
            string result = duration.ToReadableDuration();
            return result;
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

        public static string ToJson(object o)
        {
#pragma warning disable CA1869
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string result = JsonSerializer.Serialize(o, options);
            return result;
        }
    }

    public class GlobalFunctions
    {
        public static AsyncLocal<string> Url
        { get; } = new();
        public static AsyncLocal<string> BaseUrl
        { get; } = new();

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

        public static string FileNameWithoutExtension(string source)
        {
            string extension = Path.GetExtension(source);
            string filePathWithoutExt = source.Substring(0, source.Length - extension.Length);
            return filePathWithoutExt;
        }

        static string RelativeUrl(string source)
        {
            if (!string.IsNullOrWhiteSpace(BaseUrl.Value))
            {
                string result = Path.Combine($"{Path.DirectorySeparatorChar}", BaseUrl.Value, source);
                return result;
            }

            return source;
        }

        static string AbsoluteUrl(string source)
        {
            string resolvedSource = RelativeUrl(source);
            char webSeperator = '/';
            if (!string.IsNullOrWhiteSpace(resolvedSource))
            {
                if (resolvedSource.StartsWith(Path.DirectorySeparatorChar) || resolvedSource.StartsWith(webSeperator))
                {
                    resolvedSource = resolvedSource[1..];
                }

                if (!string.IsNullOrWhiteSpace(Url.Value))
                {

                    resolvedSource = $"{Url.Value}{webSeperator}{resolvedSource}";
                }
            }

            string result = resolvedSource.Replace(Path.DirectorySeparatorChar, '/');
            return result;
        }

        public static Uri AbsoluteUri(string source)
        {
            string absoluteUrl = AbsoluteUrl(source);
            Uri result = new Uri(absoluteUrl);
            return result;
        }
    }
}