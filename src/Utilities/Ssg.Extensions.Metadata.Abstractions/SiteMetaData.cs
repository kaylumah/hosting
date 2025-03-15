// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public readonly record struct SiteId(string Value)
    {
        public static implicit operator string(SiteId siteId) => siteId.Value;
        public static implicit operator SiteId(string value) => new(value);
    }

    public class SiteMetaData
    {
        Dictionary<PageId, PageMetaData> _Lookup;
        public BuildData Build
        { get; set; }
        public SiteId Id
        { get; }
        public string Title
        { get; set; }
        public string Description
        { get; set; }
        public string Language
        { get; set; }
        public string Author
        { get; set; }
        public string Url
        { get; set; }

        public Dictionary<string, object> Data
        { get; set; }

        public TagMetaDataCollection TagMetaData => GetData<TagMetaDataCollection>("tags") ?? new();
        public AuthorMetaDataCollection AuthorMetaData => GetData<AuthorMetaDataCollection>("authors") ?? new();
        public OrganizationMetaDataCollection OrganizationMetaData => GetData<OrganizationMetaDataCollection>("organizations") ?? new();

        public IDictionary<AuthorId, AuthorMetaData> Authors => AuthorMetaData.Dictionary;
        public IDictionary<OrganizationId, OrganizationMetaData> Organizations => OrganizationMetaData.Dictionary;
        public IDictionary<string, TagMetaData> Tags => TagMetaData.Dictionary;

        public List<BasePage> Items
        { get; init; }

        public IEnumerable<PageMetaData> Pages => GetPages();

        public IEnumerable<Article> RecentArticles => GetRecentArticles();

        public IEnumerable<Article> FeaturedArticles => GetFeaturedArticles();

        public IEnumerable<FacetMetaData> TagCloud => GetTagCloud();

        public SortedDictionary<string, List<PageId>> PagesByTags => GetPagesByTag();

        public SortedDictionary<int, List<PageId>> PagesByYears => GetPagesByYear();

        public SiteMetaData(
            SiteId id,
            string title,
            string description,
            string language,
            string author,
            string url,
            Dictionary<string, object> data,
            BuildData buildData,
            List<BasePage> items)
        {
            Id = id;
            Title = title;
            Description = description;
            Language = language;
            Author = author;
            Url = url;
            Data = data;
            Build = buildData;
            Items = items;

            _Lookup = GetPages()
                .ToDictionary(key => key.Id,
                              value => value);
        }

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

        public Uri AbsoluteUri(string relativeUrl)
        {
            Uri uri = RenderHelperFunctions.AbsoluteUri(Url, relativeUrl);
            return uri;
        }

        public IEnumerable<BasePage> GetItems()
        {
            return Items;
        }

        public IEnumerable<PageMetaData> GetPages()
        {
            IEnumerable<PageMetaData> pages = Items.OfType<PageMetaData>();
            return pages;
        }

        public IEnumerable<Article> GetArticles()
        {
            IEnumerable<Article> articles = Items.OfType<Article>();
            return articles;
        }

        T? GetData<T>(string key) where T : class
        {
            bool hasData = Data.TryGetValue(key, out object? value);
            if (hasData && value is T result)
            {
                return result;
            }

            return null;
        }

        IEnumerable<Article> GetRecentArticles()
        {
            IEnumerable<Article> articles = GetArticles();
            IEnumerable<Article> sortedByPublished = articles.ByRecentlyPublished();
            return sortedByPublished;
        }

        IEnumerable<Article> GetFeaturedArticles()
        {
            IEnumerable<Article> articles = GetArticles();
            IEnumerable<Article> featuredArticles = articles.IsFeatured();
            IEnumerable<Article> featuredAndSortedByPublished = featuredArticles.ByRecentlyPublished();
            return featuredAndSortedByPublished;
        }

        List<FacetMetaData> GetTagCloud()
        {
            SortedDictionary<string, List<PageId>> tags = PagesByTags;
            List<FacetMetaData> result = new List<FacetMetaData>();
            foreach (KeyValuePair<string, List<PageId>> item in tags)
            {
                string id = item.Key;
                string displayName = item.Key;
                string description = string.Empty;
                int size = 0;

                bool hasTagMetaData = TagMetaData.TryGetValue(id, out TagMetaData? tagData);
                if (hasTagMetaData && tagData != null)
                {
                    displayName = tagData.Name;
                    description = tagData.Description;
                }

                bool hasPageInfo = PagesByTags.TryGetValue(id, out List<PageId>? pageInfos);
                if (hasPageInfo && pageInfos != null)
                {
                    size = pageInfos.Count;
                }

                FacetMetaData resultForTag = new FacetMetaData(id, displayName, description, size);
                result.Add(resultForTag);
            }

            return result;
        }

        SortedDictionary<string, List<PageId>> GetPagesByTag()
        {
            SortedDictionary<string, List<PageId>> result = new(StringComparer.OrdinalIgnoreCase);

            IEnumerable<Article> articles = GetArticles();
            foreach (Article article in articles)
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

        SortedDictionary<int, List<PageId>> GetPagesByYear()
        {
            IEnumerable<Article> articles = GetArticles();
            SortedDictionary<int, List<PageId>> result = articles.GetPagesByYear();
            return result;
        }
    }

    class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T? x, T? y)
        {
            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            // Reverse the comparison for descending order
            int result = y.CompareTo(x);
            return result;
        }
    }
}