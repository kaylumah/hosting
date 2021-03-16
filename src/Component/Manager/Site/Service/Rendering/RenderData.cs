// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    class RenderData : IRenderModel, IMetadata
    {
        public BuildData Build { get; set; }
        public SiteData Site { get; set; }
        public PageData Page { get; set; }
        public string Content => Page?.Content ?? string.Empty;
        public string Title => Page?.Title ?? Site?.Title ?? null;
        public string Description => Page?.Description ?? Site?.Description ?? null;
        public string Language => Page?.Language ?? Site?.Language ?? null;
        public string Author => Page?.Author ?? Site?.Author ?? null;

    }

    class SiteData /*: Dictionary<string, object>,*/ : IMetadata, ISiteMetadata
    {
        private readonly SiteInfo _siteInfo;
        private readonly File[] _files;
        public string Id { get; set; }
        public string Title => GetTitle();
        public string Description => GetDescription();
        public string Language => GetLanguage();
        public string Author => GetAuthor();

        public SiteData(SiteInfo siteInfo, File[] files)
        {
            _siteInfo = siteInfo;
            _files = files;
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

        public Dictionary<string, object> Data { get; set; }

        public Dictionary<string, object> Collections { get; set; }

        public Dictionary<string, object> Tags { get; set; }

        public object Pages => GetPages();

        public object GetPages()
        {
            return _files
                .Where(file => ".html".Equals(Path.GetExtension(file.Name)))
                .Where(file => !"404.html".Equals(file.Name))
                .Select(x => new
                {
                    Url = x.MetaData.Uri,
                    x.LastModified,
                    Sitemap = x.MetaData["sitemap"]
                });
        }
    }

    class PageData : Dictionary<string, object>, IMetadata, IPageMetadata
    {
        public string Id
        {
            get
            {
                return this.GetValue<string>(nameof(Id));
            }
            set
            {
                this.SetValue(nameof(Id), value);
            }
        }
        public string Title => this.GetValue<string>(nameof(Title));
        public string Description => this.GetValue<string>(nameof(Description));
        public string Language => this.GetValue<string>(nameof(Language));
        public string Author => this.GetValue<string>(nameof(Author));
        public string Content
        {
            get
            {
                return this.GetValue<string>(nameof(Content));
            }
            set
            {
                this.SetValue(nameof(Content), value);
            }
        }

        public DateTimeOffset LastModified
        {
            get
            {
                return this.GetValue<DateTimeOffset>(nameof(LastModified));
            }
            set
            {
                this.SetValue(nameof(LastModified), value);
            }
        }


        public PageData(File file) : base(file.MetaData)
        {
            Content = file.Content;
            LastModified = file.LastModified;
        }
    }

    public interface IMetadata
    {
        string Title { get; }
        string Description { get; }
        string Language { get; }
        string Author { get; }
    }

    public interface IPageMetadata
    {
        string Content { get; }
    }

    public interface ISiteMetadata
    {
        Dictionary<string, object> Data { get; set; }
        Dictionary<string, object> Collections { get; set; }
        Dictionary<string, object> Tags { get; set; }
    }
}