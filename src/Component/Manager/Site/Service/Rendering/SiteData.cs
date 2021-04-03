// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class SiteData /*: Dictionary<string, object>,*/ : IMetadata, ISiteMetadata
    {
        private readonly SiteInfo _siteInfo;
        private readonly Files.Processor.File[] _files;
        public string Id { get; set; }
        public string Title => GetTitle();
        public string Description => GetDescription();
        public string Language => GetLanguage();
        public string Author => GetAuthor();

        public string Url => GetUrl();

        public SiteData(SiteInfo siteInfo, Files.Processor.File[] files)
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

        private string GetUrl()
        {
            return _siteInfo.Url;
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
}