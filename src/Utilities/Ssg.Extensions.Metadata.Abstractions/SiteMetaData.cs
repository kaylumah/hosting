// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class SiteMetaData
    {
        public BuildData Build
        { get; set; }
        public string Id
        { get; set; }
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
        { get; set; } = new();

        public TagMetaDataCollection TagMetaData
        { get; set; } = new();
        public AuthorMetaDataCollection AuthorMetaData
        { get; set; } = new();
        public OrganizationMetaDataCollection OrganizationMetaData
        { get; set; } = new();

        public List<BasePage> Items
        { get; init; }

        public SiteMetaData(string id, string title, string description, string language, string author, string url, BuildData buildData, List<BasePage> items)
        {
            Id = id;
            Title = title;
            Description = description;
            Language = language;
            Author = author;
            Url = url;
            Build = buildData;
            Items = items;
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

        public IEnumerable<Article> RecentArticles => GetRecentArticles();

        public IEnumerable<Article> FeaturedArticles => GetFeaturedArticles();

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

        // TODO Consider (re)exposing Collections, Years, Series, Types
        public SortedDictionary<string, PageMetaData[]> Tags => GetPagesByTag();
        SortedDictionary<string, PageMetaData[]> GetPagesByTag()
        {
            SortedDictionary<string, PageMetaData[]> result = new();

            List<PageMetaData> pages = GetPages().ToList();
            IEnumerable<string> tags = GetTags();

            foreach (string tag in tags)
            {
                PageMetaData[] tagFiles = pages
                    .FromTag(tag)
                    .ToArray();
                result.Add(tag, tagFiles);
            }

            return result;
        }

        public Uri AbsoluteUri(string relativeUrl)
        {
            Uri uri = RenderHelperFunctions.AbsoluteUri(Url, relativeUrl);
            return uri;
        }

        public TagViewModel GetTagViewModel(string tag)
        {
            string id = tag;
            string displayName = tag;
            int size = 0;

            bool hasTagMetaData = TagMetaData.TryGetValue(tag, out TagMetaData? tagData);
            if (hasTagMetaData && tagData != null)
            {
                displayName = tagData.Name;
            }

            TagViewModel resultForTag = new TagViewModel(tag, displayName, size);
            return resultForTag;
        }
    }
}