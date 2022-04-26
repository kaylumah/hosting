// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Engine.Transformation.Interface.Rendering;

public class SiteMetaData : ISiteMetadata
{
    private readonly SiteInfo _siteInfo;
    private readonly PageData[] _pages;
    public string Id { get; set; }
    public string Content => null;
    public string Title => GetTitle();
    public string Description => GetDescription();
    public string Language => GetLanguage();
    public string Author => GetAuthor();

    public string Url => GetUrl();

    public SiteMetaData(SiteInfo siteInfo, PageData[] pages)
    {
        _siteInfo = siteInfo;
        _pages = pages;
    }

    private string GetTitle()
    {
        return _siteInfo.Title;
    }

    private string GetDescription()
    {
        return _siteInfo.Description;
    }

    private string GetLanguage()
    {
        return _siteInfo.Lang;
    }

    private string GetAuthor()
    {
        return string.Empty;
    }

    private string GetUrl()
    {
        return _siteInfo.Url;
    }

    public Dictionary<string, object> Data { get; set; }

    public SortedDictionary<string, PageData[]> Collections { get; set; }

    public SortedDictionary<string, PageData[]> Tags { get; set; }

    public SortedDictionary<string, PageData[]> Series { get; set; }

    public SortedDictionary<int, PageData[]> Years { get; set; }

    public SortedDictionary<string, PageData[]> Types { get; set; }

    public object Pages => GetPages();

    public object GetPages()
    {
        return _pages
            .Where(file => ".html".Equals(Path.GetExtension(file.Name)))
            .Where(file => !"404.html".Equals(file.Name))
            .Select(x => new
            {
                Url = x["url"],
                x.LastModified,
                Sitemap = x["sitemap"]
            });
    }
}