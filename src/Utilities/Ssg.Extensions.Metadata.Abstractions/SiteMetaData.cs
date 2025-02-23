// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections;
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
        public IDictionary<string, TagMetaData> Tags => TagMetaData.Dictionary;

        T? GetData<T>(string key) where T : class
        {
            bool hasData = Data.TryGetValue(key, out object? value);
            if (hasData && value is T result)
            {
                return result;
            }

            return null;
        }

        public List<BasePage> Items
        { get; init; }

        public IEnumerable<PageMetaData> Pages => GetPages();

        public IEnumerable<Article> RecentArticles => GetRecentArticles();

        public IEnumerable<Article> FeaturedArticles => GetFeaturedArticles();

        public IEnumerable<TagViewModel> TagCloud => GetTagCloud();

        public SortedDictionary<string, List<PageMetaData>> PagesByTags => GetPagesByTag();

        public SortedDictionary<int, List<PageMetaData>> PagesByYears => GetPagesByYear();

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

        public IEnumerable<string> GetTags()
        {
            IEnumerable<Article> articles = GetArticles();
            IEnumerable<PageMetaData> taggedArticles = articles.HasTag();
            IEnumerable<string> tagsFromArticles = taggedArticles.SelectMany(article => article.Tags);
            HashSet<string> uniqueTags = new HashSet<string>(tagsFromArticles);
            IEnumerable<string> result = uniqueTags.Order();
            return result;
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

        List<TagViewModel> GetTagCloud()
        {
            SortedDictionary<string, List<PageMetaData>> tags = PagesByTags;
            List<TagViewModel> result = new List<TagViewModel>();
            foreach (KeyValuePair<string, List<PageMetaData>> item in tags)
            {
                string tag = item.Key;
                TagViewModel resultForTag = GetTagViewModel(tag);
                result.Add(resultForTag);
            }

            return result;
        }

        public TagViewModel GetTagViewModel(string tag)
        {
            string id = tag;
            string displayName = tag;
            string description = string.Empty;
            int size = 0;

            bool hasTagMetaData = TagMetaData.TryGetValue(id, out TagMetaData? tagData);
            if (hasTagMetaData && tagData != null)
            {
                displayName = tagData.Name;
                description = tagData.Description;
            }

            bool hasPageInfo = PagesByTags.TryGetValue(id, out List<PageMetaData>? pageInfos);
            if (hasPageInfo && pageInfos != null)
            {
                size = pageInfos.Count;
            }

            TagViewModel resultForTag = new TagViewModel(id, displayName, description, size);
            return resultForTag;
        }

        public PageMetaData this[PageId pageId]
        {
            /*
             *
             * public PageMetaData? this[Guid id]
               {
                   get => _pageLookup.TryGetValue(id, out var page) ? page : null;
               }
             */

            get
            {
                // Lookup will be more efficient...
                IEnumerable<PageMetaData> pages = GetPages();
                PageMetaData? page = pages.SingleOrDefault(x => x.Id == pageId);
                if (page == null)
                {
                    throw new KeyNotFoundException();
                }

                return page;
            }
        }

        SortedDictionary<string, List<PageMetaData>> GetPagesByTag()
        {
            SortedDictionary<string, List<PageMetaData>> result = new();

            List<PageMetaData> pages = GetPages().ToList();
            IEnumerable<string> tags = GetTags();

            foreach (string tag in tags)
            {
                List<PageMetaData> tagFiles = pages
                    .FromTag(tag)
                    .ToList();
                result.Add(tag, tagFiles);
            }

            return result;
        }

        SortedDictionary<int, List<PageMetaData>> GetPagesByYear()
        {
            SortedDictionary<int, List<PageMetaData>> result = new();
            IEnumerable<Article> articles = GetArticles();
            foreach (Article article in articles)
            {
                DateTimeOffset published = article.Published;
                int year = published.Year;
                if (result.ContainsKey(year) == false)
                {
                    result[year] = new();
                }

                result[year].Add(article);
            }

            return result;
        }
    }
}