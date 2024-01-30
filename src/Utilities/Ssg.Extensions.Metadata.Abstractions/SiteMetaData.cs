// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class SiteMetaData
    {
        public TagMetaDataCollection TagMetaData
        { get; set; } = new();
        public AuthorMetaDataCollection AuthorMetaData
        { get; set; } = new();
        public OrganizationMetaDataCollection OrganizationMetaData
        { get; set; } = new();
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
        { get; set; }

        public SortedDictionary<string, PageMetaData[]> Collections
        { get; set; }

        public SortedDictionary<string, PageMetaData[]> Tags
        { get; set; }

        public SortedDictionary<string, PageMetaData[]> Series
        { get; set; }

        public SortedDictionary<int, PageMetaData[]> Years
        { get; set; }

        public SortedDictionary<string, PageMetaData[]> Types
        { get; set; }

        public List<PageMetaData> Pages
        { get; set; }
    }
}
