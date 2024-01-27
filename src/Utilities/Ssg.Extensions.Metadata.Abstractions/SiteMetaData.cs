// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class SiteMetaData
    {
        public TagMetaDataCollection TagMetaData { get; set; } = new();
        public AuthorMetaDataCollection AuthorMetaData { get; set; } = new();
        public OrganizationMetaDataCollection OrganizationMetaData { get; set; } = new();
        public BuildData? Build { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public Dictionary<string, object> Data { get; set; } = new();

        public SortedDictionary<string, PageMetaData[]> Collections { get; set; } = new();

        public SortedDictionary<string, PageMetaData[]> Tags { get; set; } = new();

        public SortedDictionary<string, PageMetaData[]> Series { get; set; } = new();

        public SortedDictionary<int, PageMetaData[]> Years { get; set; } = new();

        public SortedDictionary<string, PageMetaData[]> Types { get; set; } = new();

        public List<PageMetaData> Pages { get; set; } = new();
    }
}
