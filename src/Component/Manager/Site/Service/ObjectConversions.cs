// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class ObjectConversions
    {
        static readonly JsonSerializerOptions _JsonSerializerOptions;

        static ObjectConversions()
        {
            AuthorIdJsonConverter authorIdJsonConverter = new AuthorIdJsonConverter();
            OrganizationIdJsonConverter organizationIdJsonConverter = new OrganizationIdJsonConverter();
            _JsonSerializerOptions = new JsonSerializerOptions();
            _JsonSerializerOptions.WriteIndented = true;
            _JsonSerializerOptions.Converters.Add(authorIdJsonConverter);
            _JsonSerializerOptions.Converters.Add(organizationIdJsonConverter);
        }

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
            SortedDictionary<string, PageMetaData[]> tags = site.PagesByTags;
            List<TagViewModel> result = new List<TagViewModel>();
            foreach (KeyValuePair<string, PageMetaData[]> item in tags)
            {
                string tag = item.Key;
                TagViewModel resultForTag = GetTag(site, tag);
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

            bool tagExists = site.PagesByTags.TryGetValue(tag, out PageMetaData[]? resultForTag);
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
            string result = JsonSerializer.Serialize(o, _JsonSerializerOptions);
            return result;
        }

        public static string ToDiagnosticHtml(object o, string id)
        {
            string json = ToJson(o);
            StringBuilder sb = new StringBuilder();
            sb.Append(CultureInfo.InvariantCulture, $"<pre id=\"{id}\">");
            sb.Append(json);
            sb.Append("</pre>");
            string result = sb.ToString();
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