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

        protected int GetInt(string key)
        {
            int result = _InternalData.GetValue<int>(key);
            return result;
        }

        protected TimeSpan GetTimeSpan(string key)
        {
            TimeSpan result = _InternalData.GetValue<TimeSpan>(key);
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

        public Uri Canonical => GetCanonicalUri();

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

        Uri GetCanonicalUri()
        {
            Uri result;
            if ("index.html".Equals(Uri, StringComparison.OrdinalIgnoreCase))
            {
                string baseUrl = RenderHelperFunctions.Url.Value!;
                result = new Uri(baseUrl);
            }
            else
            {
                result = RenderHelperFunctions.AbsoluteUri(Uri);
            }

            return result;
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
        public Uri? WebImage => ResolveImageUri();

        Uri? ResolveImageUri()
        {
            string? image = Image;
            if (string.IsNullOrEmpty(image))
            {
                return null;
            }

            Uri result = RenderHelperFunctions.AbsoluteUri(image);
            return result;
        }

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
        public DateTimeOffset Modified => GetModifiedDate();

        protected virtual DateTimeOffset GetPublishedDate()
        {
            DateTimeOffset result = GetDateTimeOffsetValue(nameof(Published));
            return result;
        }

        protected virtual DateTimeOffset GetModifiedDate()
        {
            DateTimeOffset result = GetDateTimeOffsetValue(nameof(Modified));
            return result;
        }
    }

    public class CollectionPage : PageMetaData
    {
        readonly IEnumerable<Article> _Articles;

        public CollectionPage(PageMetaData internalData, IEnumerable<Article> articles) : base(internalData)
        {
            _Articles = articles.ByRecentlyPublished();
        }

        protected override DateTimeOffset GetPublishedDate()
        {
            Article? firstPublishedArticle = _Articles.LastOrDefault();
            DateTimeOffset result;
            if (firstPublishedArticle != null)
            {
                result = firstPublishedArticle.Published;
            }
            else
            {
                result = base.GetPublishedDate();
            }

            return result;
        }

        protected override DateTimeOffset GetModifiedDate()
        {
            Article? lastPublishedArticle = _Articles.FirstOrDefault();
            DateTimeOffset result;
            if (lastPublishedArticle != null)
            {
                result = lastPublishedArticle.Published;
            }
            else
            {
                // note: if there are no articles use Published date for both fields in collection
                result = base.GetPublishedDate();
            }

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

        public int NumberOfWords
        {
            get
            {
                int result = GetInt(nameof(NumberOfWords));
                return result;
            }
            set
            {
                SetValue(nameof(NumberOfWords), value);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan result = GetTimeSpan(nameof(Duration));
                return result;
            }
            set
            {
                SetValue(nameof(Duration), value);
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
            // TODO use this more
            bool result = url.Equals(pageMetaData.Uri, StringComparison.OrdinalIgnoreCase);
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

        public static bool IsCollection(this PageMetaData page)
        {
            bool result = page.IsContentType("Collection");
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

        public static IEnumerable<Article> ByRecentlyPublished(this IEnumerable<Article> source)
        {
            IOrderedEnumerable<Article> result = source.OrderByDescending(x => x.Published);
            return result;
        }
    }
}
