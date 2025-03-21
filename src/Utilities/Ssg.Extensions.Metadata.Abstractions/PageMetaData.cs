﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
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
            string? result = _InternalData.GetValue<string>(key);
            // TODO fix NULL suppression
            return result!;
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
            bool result = _InternalData.GetValue<bool>(key);
            return result;
        }

        protected DateTimeOffset GetDateTimeOffsetValue(string key)
        {
            DateTimeOffset result = _InternalData.GetValue<DateTimeOffset>(key);
            return result;
        }

        protected List<string> GetStringValues(string key)
        {
            IEnumerable<string>? values = _InternalData.GetValues<string>(key);
            List<string> result = values?.ToList() ?? new List<string>();
            return result;
        }

        protected void SetValue(string key, object? value)
        {
            _InternalData.SetValue(key, value);
        }

        public string Uri => GetString(nameof(Uri));

        public string BaseUri => GetString(nameof(BaseUri));

        public Uri CanonicalUri => GetCanonicalUri();

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
            string baseUrl = BaseUri;
            Uri result;
            if ("index.html".Equals(Uri, StringComparison.OrdinalIgnoreCase))
            {
                result = new Uri(baseUrl);
            }
            else
            {
                result = RenderHelperFunctions.AbsoluteUri(baseUrl, Uri);
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

    public readonly record struct PageId(string Value)
    {
        public static implicit operator string(PageId pageId) => pageId.Value;
        public static implicit operator PageId(string value) => new(value);
    }

    public class PageMetaData : BasePage
    {
        public PageMetaData(Dictionary<string, object?> internalData) : base(internalData)
        {
        }

        public PageId Id
        {
            get
            {
                string result = GetString(nameof(Id));
                return result;
            }
            set
            {
                string strValue = value;
                SetValue(nameof(Id), strValue);
            }
        }
        public string Title => GetString(nameof(Title));
        public string Description => GetString(nameof(Description));
        public string Language => GetString(nameof(Language));
        public AuthorId Author => GetString(nameof(Author));
        public OrganizationId Organization => GetString(nameof(Organization));
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

            Uri result = RenderHelperFunctions.AbsoluteUri(BaseUri, image);
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
        readonly Dictionary<PageId, PageMetaData> _Lookup;

        public int Take => GetInt(nameof(Take));

        public IEnumerable<BasePage> Items
        { get; }

        public IEnumerable<PageMetaData> Pages => GetPages();

        public IEnumerable<Article> RecentArticles => GetRecentArticles();

        public SortedDictionary<int, List<PageId>> PagesByYears => GetPagesByYear();

        public CollectionPage(PageMetaData internalData, List<BasePage> items) : base(internalData)
        {
            Items = items;

            _Lookup = GetPages()
                .ToDictionary(key => key.Id,
                    value => value);
        }

        #region Indexers

        public PageMetaData? this[PageId pageId]
        {
            get => _Lookup.GetValueOrDefault(pageId);
        }

        public IEnumerable<PageMetaData> this[IEnumerable<PageId> ids]
        {
            get
            {
                foreach (PageId id in ids)
                {
                    if (_Lookup.TryGetValue(id, out PageMetaData? page))
                    {
                        yield return page;
                    }
                }
            }
        }

        #endregion

        protected override DateTimeOffset GetPublishedDate()
        {
            IEnumerable<PageMetaData> pages = GetPages();
            // if there is a published date it should win? otherwise fall back on the oldest page
            PageMetaData? firstPublishedPage = pages.LastOrDefault();
            DateTimeOffset basePublishedDate = base.GetPublishedDate();
            DateTimeOffset? firstPublishedDate = firstPublishedPage?.Published;
            DateTimeOffset result = firstPublishedDate ?? basePublishedDate;
            return result;
        }

        protected override DateTimeOffset GetModifiedDate()
        {
            IEnumerable<PageMetaData> pages = GetPages();
            // if there is a newer published date it should win? otherwise fall back on newest page
            PageMetaData? lastPublishedPage = pages.FirstOrDefault();
            // DateTimeOffset defaultDate = DateTimeOffset.MinValue;
            DateTimeOffset baseModifiedDate = base.GetModifiedDate();
            DateTimeOffset? lastPublishedDate = lastPublishedPage?.Published;

            DateTimeOffset result;
            /*
            if (lastPublishedDate == null || lastPublishedDate == defaultDate)
            {
                result = baseModifiedDate;
            }

            if (baseModifiedDate == defaultDate)
            {
                // return lastPublishedDate
            }
            */

            if (baseModifiedDate < lastPublishedDate)
            {
                result = lastPublishedDate.Value;
            }
            else
            {
                result = baseModifiedDate;
            }

            return result;
        }

        #region PageTypes

        IEnumerable<PageMetaData> GetPages()
        {
            IEnumerable<PageMetaData> pages = Items.OfType<PageMetaData>()
                .ByRecentlyPublished();

            if (0 < Take)
            {
                pages = pages.Take(Take);
            }

            return pages;
        }

        IEnumerable<Article> GetArticles()
        {
            IEnumerable<Article> articles = Items.OfType<Article>()
                .ByRecentlyPublished();

            if (0 < Take)
            {
                articles = articles.Take(Take);
            }

            return articles;
        }

        #endregion

        #region Collections

        IEnumerable<Article> GetRecentArticles()
        {
            IEnumerable<Article> articles = GetArticles();
            IEnumerable<Article> sortedByPublished = articles.ByRecentlyPublished();
            return sortedByPublished;
        }

        #endregion

        #region CrossSections

        SortedDictionary<int, List<PageId>> GetPagesByYear()
        {
            IEnumerable<Article> articles = GetArticles();
            SortedDictionary<int, List<PageId>> result = articles.GetPagesByYear();
            return result;
        }

        #endregion
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

        public static IEnumerable<PageMetaData> ByRecentlyPublished(this IEnumerable<PageMetaData> source)
        {
            IOrderedEnumerable<PageMetaData> result = source.OrderByDescending(x => x.Published);
            return result;
        }

        public static SortedDictionary<string, List<PageId>> GetPagesByTag(this IEnumerable<Article> source)
        {
            SortedDictionary<string, List<PageId>> result = new(StringComparer.OrdinalIgnoreCase);
            foreach (Article article in source)
            {
                List<string> tags = article.Tags;
                foreach (string tag in tags)
                {
                    if (result.ContainsKey(tag) == false)
                    {
                        result[tag] = new();
                    }

                    result[tag].Add(article.Id);
                }
            }

            return result;
        }

        public static SortedDictionary<int, List<PageId>> GetPagesByYear(this IEnumerable<Article> source)
        {
            DescendingComparer<int> comparer = new DescendingComparer<int>();
            SortedDictionary<int, List<PageId>> result = new(comparer);
            foreach (Article article in source)
            {
                DateTimeOffset published = article.Published;
                int year = published.Year;
                if (result.ContainsKey(year) == false)
                {
                    result[year] = new();
                }

                result[year].Add(article.Id);
            }

            return result;
        }
    }
}
