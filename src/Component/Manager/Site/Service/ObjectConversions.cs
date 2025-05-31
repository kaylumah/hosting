// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    [ExcludeFromCodeCoverage]
    public class ObjectConversions
    {
        static readonly JsonSerializerOptions _JsonSerializerOptions;

        static ObjectConversions()
        {
            _JsonSerializerOptions = new JsonSerializerOptions();
            _JsonSerializerOptions.WriteIndented = true;
            StronglyTypedIdJsonConverter<AuthorId> authorIdJsonConverter = new StronglyTypedIdJsonConverter<AuthorId>();
            StronglyTypedIdJsonConverter<OrganizationId> organizationIdJsonConverter = new StronglyTypedIdJsonConverter<OrganizationId>();
            StronglyTypedIdJsonConverter<TagId> tagIdJsonConverter = new StronglyTypedIdJsonConverter<TagId>();
            StronglyTypedIdJsonConverter<PageId> pageIdJsonConverter = new StronglyTypedIdJsonConverter<PageId>();
            StronglyTypedIdJsonConverter<SiteId> siteIdJsonConverter = new StronglyTypedIdJsonConverter<SiteId>();
            _JsonSerializerOptions.Converters.Add(authorIdJsonConverter);
            _JsonSerializerOptions.Converters.Add(organizationIdJsonConverter);
            _JsonSerializerOptions.Converters.Add(tagIdJsonConverter);
            _JsonSerializerOptions.Converters.Add(pageIdJsonConverter);
            _JsonSerializerOptions.Converters.Add(siteIdJsonConverter);
        }

        public static string PublishedTimeAgo(SiteMetaData siteMetaData, ArticlePublicationPageMetaData pagePublicationPageMetaData)
        {
            Debug.Assert(pagePublicationPageMetaData != null);
            Debug.Assert(siteMetaData != null);
            Debug.Assert(siteMetaData.Build != null);
            BuildData buildData = siteMetaData.Build;
            DateTimeOffset buildTime = buildData.Time;
            DateTimeOffset publishedTime = pagePublicationPageMetaData.Published;
            string result = buildTime.ToReadableRelativeTime(publishedTime);
            return result;
        }

        public static object? GetValue(BasePage basePage, string key)
        {
            Dictionary<string, object?> dictionary = basePage;
            object? result = dictionary.GetValue<object?>(key);
            return result;
        }

        public static string ReadingTime(ArticlePublicationPageMetaData pagePublicationPageMetaData)
        {
            TimeSpan duration = pagePublicationPageMetaData.Duration;
            string result = duration.ToReadableDuration();
            return result;
        }

        public static IEnumerable<FacetMetaData> TagCloud(SiteMetaData site)
        {
            IEnumerable<FacetMetaData> result = site.TagCloud;
            return result;
        }

        public static Uri AbsoluteUri(SiteMetaData site, string relativeUrl)
        {
            Uri result = site.AbsoluteUri(relativeUrl);
            return result;
        }

        public static IEnumerable<ArticlePublicationPageMetaData> ArticlesForTag(SiteMetaData site, string tag, int? take = null)
        {
            ArgumentNullException.ThrowIfNull(site);
            ArgumentNullException.ThrowIfNull(tag);

            bool tagExists = site.PagesByTags.TryGetValue(tag, out List<PageId>? idsForTag);
            IEnumerable<ArticlePublicationPageMetaData> result;
            if (tagExists && idsForTag != null)
            {
                IEnumerable<PageMetaData> resultForTag = site[idsForTag];
                IEnumerable<ArticlePublicationPageMetaData> asArticles = resultForTag.OfType<ArticlePublicationPageMetaData>();
                result = asArticles.ByRecentlyPublished();
                if (take != null)
                {
                    result = result.Take(take.Value);
                }
            }
            else
            {
                result = Enumerable.Empty<ArticlePublicationPageMetaData>();
            }

            return result;
        }

        public static string ToJson(object o)
        {
            string result = JsonSerializer.Serialize(o, _JsonSerializerOptions);
            return result;
        }

        public static string ToDiagnosticHtml(object o, string id = "preview")
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