// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Utilities;
using Ssg.Extensions.Metadata.Abstractions;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class SiteTests
    {
        [Fact]
        public void TagCloud_Empty()
        {
            SiteMetaData site = CreateSiteMetaData();
            IEnumerable<TagViewModel> result = ObjectConversions.TagCloud(site);
        }
        
        [Fact]
        public void TagCloud_NoTags()
        {
            List<BasePage> items = new List<BasePage>();
            PageMetaData pageMetaData = CreatePageMetaData();
            items.Add(pageMetaData);
            SiteMetaData site = CreateSiteMetaData(items);
            IEnumerable<TagViewModel> result = ObjectConversions.TagCloud(site);
        }
        
        static PageMetaData CreatePageMetaData()
        {
            Dictionary<string, object> data = new();
            PageMetaData result = new PageMetaData(data);
            return result;
        }
        
        static SiteMetaData CreateSiteMetaData(List<BasePage> items = null)
        {
            string id = string.Empty;
            string title = string.Empty;
            string description = string.Empty;
            string language = string.Empty;
            string author = string.Empty;
            string url = string.Empty;
            Dictionary<string,object> data = new();
            BuildData build = EnrichSiteWithAssemblyData();
            items ??= new List<BasePage>();
            SiteMetaData result = new SiteMetaData(id, title, description, language, author, url, data, build, items);
            return result;
        }

        static BuildData EnrichSiteWithAssemblyData()
        {
#pragma warning disable
            AssemblyInfo assemblyInfo = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
            DateTimeOffset localNow = DateTimeOffset.Now;
            BuildData buildMetadata = new BuildData(assemblyInfo, localNow);
            return buildMetadata;
#pragma warning restore
        }
    }
}