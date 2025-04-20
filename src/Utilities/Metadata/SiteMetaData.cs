// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public class SiteMetaData
    {
        readonly Dictionary<PageId, PageMetaData> _Lookup;
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
        public BuildData Build
        { get; set; }
        public Dictionary<string, object> Data
        { get; set; }

        public TagMetaDataCollection TagMetaData => GetData<TagMetaDataCollection>("tags") ?? new();
        public IDictionary<string, TagMetaData> Tags => TagMetaData.Dictionary;

        public AuthorMetaDataCollection AuthorMetaData => GetData<AuthorMetaDataCollection>("authors") ?? new();
        public IDictionary<AuthorId, AuthorMetaData> Authors => AuthorMetaData.Dictionary;

        public OrganizationMetaDataCollection OrganizationMetaData => GetData<OrganizationMetaDataCollection>("organizations") ?? new();
        public IDictionary<OrganizationId, OrganizationMetaData> Organizations => OrganizationMetaData.Dictionary;

        public IEnumerable<BasePage> Items
        { get; }

        public IEnumerable<PageMetaData> Pages => GetPages();

        public IEnumerable<ArticlePublicationPageMetaData> RecentArticles => GetRecentArticles();

        public IEnumerable<ArticlePublicationPageMetaData> FeaturedArticles => GetFeaturedArticles();

        public SortedDictionary<string, List<PageId>> PagesByTags => GetPagesByTag();

        public SortedDictionary<int, List<PageId>> PagesByYears => GetPagesByYear();

        public IEnumerable<FacetMetaData> TagCloud => GetTagCloud();

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

        public Uri AbsoluteUri(string relativeUrl)
        {
            Uri uri = RenderHelperFunctions.AbsoluteUri(Url, relativeUrl);
            return uri;
        }

        #region PageTypes

        IEnumerable<PageMetaData> GetPages()
        {
            IEnumerable<PageMetaData> pages = Items.OfType<PageMetaData>();
            return pages;
        }

        IEnumerable<ArticlePublicationPageMetaData> GetArticles()
        {
            IEnumerable<ArticlePublicationPageMetaData> articles = Items.OfType<ArticlePublicationPageMetaData>();
            return articles;
        }

        IEnumerable<PublicationPageMetaData> GetPublications()
        {
            IEnumerable<PublicationPageMetaData> items = Items.OfType<PublicationPageMetaData>();
            return items;
        }

        #endregion

        #region Collections

        IEnumerable<ArticlePublicationPageMetaData> GetRecentArticles()
        {
            IEnumerable<ArticlePublicationPageMetaData> articles = GetArticles();
            IEnumerable<ArticlePublicationPageMetaData> sortedByPublished = articles.ByRecentlyPublished();
            return sortedByPublished;
        }

        IEnumerable<ArticlePublicationPageMetaData> GetFeaturedArticles()
        {
            IEnumerable<ArticlePublicationPageMetaData> articles = GetArticles();
            IEnumerable<ArticlePublicationPageMetaData> featuredArticles = articles.IsFeatured();
            IEnumerable<ArticlePublicationPageMetaData> featuredAndSortedByPublished = featuredArticles.ByRecentlyPublished();
            return featuredAndSortedByPublished;
        }

        #endregion

        #region CrossSections

        SortedDictionary<string, List<PageId>> GetPagesByTag()
        {
            IEnumerable<PublicationPageMetaData> items = GetPublications();
            SortedDictionary<string, List<PageId>> result = items.GetPagesByTag();
            return result;
        }

        SortedDictionary<int, List<PageId>> GetPagesByYear()
        {
            IEnumerable<PublicationPageMetaData> items = GetPublications();
            SortedDictionary<int, List<PageId>> result = items.GetPagesByYear();
            return result;
        }

        #endregion

        T? GetData<T>(string key) where T : class
        {
            bool hasData = Data.TryGetValue(key, out object? value);
            if (hasData && value is T result)
            {
                return result;
            }

            return null;
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
    }
}