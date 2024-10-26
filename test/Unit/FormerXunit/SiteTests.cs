// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.Empty(result);
        }

        [Fact]
        public void TagCloud_NoTags()
        {
            List<BasePage> items = new List<BasePage>();
            BasePage pageMetaData = CreatePageMetaData(new());
            items.Add(pageMetaData);
            SiteMetaData site = CreateSiteMetaData(items);
            IEnumerable<TagViewModel> result = ObjectConversions.TagCloud(site);
            Assert.Empty(result);
        }

        [Fact]
        public void TagCloud_SingleTag_NoData()
        {
            List<BasePage> items = new List<BasePage>();
            Dictionary<string, object> data = new();
            BasePage pageMetaData = CreatePageMetaData(data);
            data["tags"] = new List<object>() { "tag1" };
            items.Add(pageMetaData);
            SiteMetaData site = CreateSiteMetaData(items);
            IEnumerable<TagViewModel> result = ObjectConversions.TagCloud(site);
            Assert.Single(result);

            TagViewModel tagViewModel = result.Single();
            Assert.Empty(tagViewModel.Description);
        }

        [Fact]
        public void TagCloud_SingleTag_WithData()
        {
            List<BasePage> items = new List<BasePage>();
            Dictionary<string, object> data = new();
            BasePage pageMetaData = CreatePageMetaData(data);
            data["tags"] = new List<object>() { "tag1" };
            items.Add(pageMetaData);

            TagMetaDataCollection tagMetaDataCollection = new();
            tagMetaDataCollection.Add(new TagMetaData() { Id = "tag1", Description = "t" });
            Dictionary<string, object> dataItem = new()
            {
                ["tags"] = tagMetaDataCollection
            };

            SiteMetaData site = CreateSiteMetaData(items, dataItem);
            IEnumerable<TagViewModel> result = ObjectConversions.TagCloud(site);
            Assert.Single(result);

            TagViewModel tagViewModel = result.Single();
            Assert.NotEmpty(tagViewModel.Description);
        }

        static BasePage CreatePageMetaData(Dictionary<string, object> data)
        {
            Article result = new Article(data);
            return result;
        }

        static SiteMetaData CreateSiteMetaData(List<BasePage> items = null, Dictionary<string, object> data = null)
        {
            string id = string.Empty;
            string title = string.Empty;
            string description = string.Empty;
            string language = string.Empty;
            string author = string.Empty;
            string url = string.Empty;
            data ??= new();
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