// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("PageMetaData '{Uri}'")]
    public abstract class BasePage
    {
        public static implicit operator Dictionary<string, object?>(BasePage page) => page._InternalData;
        readonly Dictionary<string, object?> _InternalData;

        public BasePage(Dictionary<string, object?> internalData)
        {
            _InternalData = internalData;
        }

        protected string GetString(string key)
        {
            string result = _InternalData.GetValue<string>(key);
            return result;
        }

        protected bool GetBoolValue(string key)
        {
            bool result = _InternalData.GetBoolValue(key);
            return result;
        }

        protected DateTimeOffset GetDateTimeOffsetValue(string key)
        {
            DateTimeOffset result = _InternalData.GetValue<DateTimeOffset>(key);
            return result;
        }

        protected List<string> GetStringValues(string key)
        {
            List<string> result = _InternalData.GetStringValues(key);
            return result;
        }

        protected void SetValue(string key, object? value)
        {
            _InternalData.SetValue(key, value);
        }

        public string Uri => GetString(nameof(Uri));

        public string Content
        {
            get
            {
                string result = GetString(nameof(Content));
                return result;
            }
            set
            {
                SetValue(nameof(Content), value);
            }
        }

        public string Type
        {
            get
            {
                string result = GetString(nameof(Type));
                return result;
            }
            set
            {
                SetValue(nameof(Type), value);
            }
        }
    }

    public class StaticContent : BasePage
    {
        public StaticContent(Dictionary<string, object?> internalData) : base(internalData)
        {
        }
    }

    public class PageMetaData : BasePage
    {
        public PageMetaData(Dictionary<string, object?> internalData) : base(internalData)
        {
        }

        public string Id
        {
            get
            {
                string result = GetString(nameof(Id));
                return result;
            }
            set
            {
                SetValue(nameof(Id), value);
            }
        }
        public string Title => GetString(nameof(Title));
        public string Description => GetString(nameof(Description));
        public string Language => GetString(nameof(Language));
        public string Author => GetString(nameof(Author));
        public string Organization => GetString(nameof(Organization));
        public bool Sitemap => GetBoolValue(nameof(Sitemap));

        public string LdJson
        {
            get
            {
                string result = GetString(nameof(LdJson));
                return result;
            }
            set
            {
                SetValue(nameof(LdJson), value);
            }
        }
        public string MetaTags
        {
            get
            {
                string result = GetString(nameof(MetaTags));
                return result;
            }
            set
            {
                SetValue(nameof(MetaTags), value);
            }
        }
        public string Layout => GetString(nameof(Layout));
        public string Image => GetString(nameof(Image));

        public string Name
        {
            get
            {
                string result = GetString(nameof(Name));
                return result;
            }
            set
            {
                SetValue(nameof(Name), value);
            }
        }

        public string Collection
        {
            get
            {
                string result = GetString(nameof(Collection));
                return result;
            }
            set
            {
                SetValue(nameof(Collection), value);
            }
        }

        public List<string> Tags
        {
            get
            {
                List<string>? tags = GetStringValues(nameof(Tags));
                return tags;
            }
            set
            {
                SetValue(nameof(Tags), value);
            }
        }

        public DateTimeOffset Published => GetPublishedDate();
        public DateTimeOffset Modified => GetDateTimeOffsetValue(nameof(Modified));

        protected virtual DateTimeOffset GetPublishedDate()
        {
            DateTimeOffset result = GetDateTimeOffsetValue(nameof(Published));
            return result;
        }
    }

    public class Article : PageMetaData
    {
        public bool SocialShare => GetBoolValue(nameof(SocialShare));
        public bool Feed => GetBoolValue(nameof(Feed));
        public bool Featured => GetBoolValue(nameof(Featured));
        public string CommentId => GetString(nameof(CommentId));

        public string Series
        {
            get
            {
                string result = GetString(nameof(Series));
                return result;
            }
            set
            {
                SetValue(nameof(Series), value);
            }
        }

        public Article(Dictionary<string, object?> internalData) : base(internalData)
        {
        }
    }

    public static class PageMetaDataExtensions
    {
        public static readonly Func<PageMetaData, bool> Tags;

        public static readonly Func<Article, bool> Series;

        public static readonly Func<PageMetaData, bool> Html;

        public static readonly Func<Article, bool> Featured;

        static PageMetaDataExtensions()
        {
            Tags = (page) => page.Tags != null && 0 < page.Tags.Count;
            Series = (page) => !string.IsNullOrEmpty(page.Series);
            Html = (page) => page.IsHtml();
            Featured = (page) => page.Featured;
        }

        public static string GetExtension(this PageMetaData pageMetaData)
        {
            string result = Path.GetExtension(pageMetaData.Uri);
            return result;
        }

        public static bool IsExtension(this PageMetaData pageMetaData, string target)
        {
            ArgumentNullException.ThrowIfNull(target);
            string actual = pageMetaData.GetExtension();
            bool result = target.Equals(actual, StringComparison.Ordinal);
            return result;
        }

        public static bool IsUrl(this PageMetaData pageMetaData, string url)
        {
            bool result = url.Equals(pageMetaData.Uri, StringComparison.Ordinal);
            return result;
        }

        public static bool IsHtml(this PageMetaData pageMetaData)
        {
            bool result = pageMetaData.IsExtension(".html");
            return result;
        }

        public static IEnumerable<PageMetaData> HasTag(this IEnumerable<PageMetaData> source)
        {
            IEnumerable<PageMetaData> result = source.Where(Tags);
            return result;
        }

        public static IEnumerable<PageMetaData> FromTag(this IEnumerable<PageMetaData> source, string tag)
        {
            IEnumerable<PageMetaData> result = source
                    .HasTag()
                    .Where(page => page.Tags.Contains(tag));
            return result;
        }

        public static IEnumerable<Article> HasSeries(this IEnumerable<Article> source)
        {
            IEnumerable<Article> result = source.Where(Series);
            return result;
        }

        public static IEnumerable<Article> FromSeries(this IEnumerable<Article> source, string series)
        {
            IEnumerable<Article> result = source
                    .HasSeries()
                    .Where(page => page.Series.Equals(series, StringComparison.Ordinal));
            return result;
        }

        public static bool IsContentType(this PageMetaData page, string contentType)
        {
            bool result = page.Type != null && page.Type.Equals(contentType, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsArticle(this PageMetaData page)
        {
            bool result = page.IsContentType("Article");
            return result;
        }

        public static bool IsPage(this PageMetaData page)
        {
            bool result = page.IsContentType("Page");
            return result;
        }

        public static IEnumerable<PageMetaData> IsArticle(this IEnumerable<PageMetaData> source)
        {
            IEnumerable<PageMetaData> result = source.Where(IsArticle);
            return result;
        }

        public static IEnumerable<Article> IsFeatured(this IEnumerable<Article> source)
        {
            IEnumerable<Article> result = source.Where(Featured);
            return result;
        }

        public static IEnumerable<PageMetaData> ByRecentlyPublished(this IEnumerable<PageMetaData> source)
        {
            IOrderedEnumerable<PageMetaData> result = source.OrderByDescending(x => x.Published);
            return result;
        }
    }
}
