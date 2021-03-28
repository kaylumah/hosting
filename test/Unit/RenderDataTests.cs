// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Reflection;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Xunit;

namespace Test.Unit
{
    public class RenderDataTests
    {
        // [Fact]
        // public void TestRenderData()
        // {
        //     var siteInfo = new SiteInfo();
        //     var file = new File() {
        //         MetaData = new FileMetaData {}
        //     };
        //     var siteData = new SiteData(siteInfo, new File[] { file });
        //     var pageData = new PageData(file);
        //     var sut = new RenderData { Site = siteData, Page = pageData };
        //     sut.Should().NotBeNull();
        // }

        [Fact]
        public void Test_RenderData_ContentEqualsPageContentIfNotNull()
        {
            var siteInfo = new SiteInfo();
            var file = new File() {
                MetaData = new FileMetaData {
                },
                Content = "-"
            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Content.Should().Be(file.Content);
        }

        [Fact]
        public void Test_RenderData_ContentEqualsEmptyStringWhenPageContentIsNull()
        {
            var siteInfo = new SiteInfo();
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                },
                Content = null
            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Content.Should().Be(string.Empty);
        }

        [Fact]
        public void Test_RenderData_ContentEqualsEmptyStringWhenPageIsNull()
        {
            var siteInfo = new SiteInfo();
            var siteData = new SiteData(siteInfo, new File[] { });
            var sut = new RenderData { Site = siteData, Page = null };
            sut.Should().NotBeNull();
            sut.Content.Should().Be(string.Empty);
        }
    }
}