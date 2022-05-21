// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

public class SiteMetaData
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public string Author { get; set; }
    public string Url { get; set; }

    public Dictionary<string, object> Data { get; set; }

    public SortedDictionary<string, PageMetaData[]> Collections { get; set; }

    public SortedDictionary<string, PageMetaData[]> Tags { get; set; }

    public SortedDictionary<string, PageMetaData[]> Series { get; set; }

    public SortedDictionary<int, PageMetaData[]> Years { get; set; }

    public SortedDictionary<string, PageMetaData[]> Types { get; set; }

    public object Pages { get; set; }
}
