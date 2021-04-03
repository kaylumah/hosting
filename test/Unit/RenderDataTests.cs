// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Reflection;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
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
        public void Test_RenderData_TitleEqualsPageTitleIfExists()
        {
            var siteInfo = new SiteInfo();
            var file = new File() {
                MetaData = new FileMetaData {
                    { "title", "1" }
                }
                
            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Title.Should().Be("1");
        }

        [Fact]
        public void Test_RenderData_TitleEqualsSiteTitleIfExists()
        {
            var siteInfo = new SiteInfo() {
                Title = "2"
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Title.Should().Be("2");
        }

        [Fact]
        public void Test_RenderData_TitleEqualsNullIfPageAndSiteAreNull()
        {
            var siteInfo = new SiteInfo()
            {
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Title.Should().BeNull();
        }

        [Fact]
        public void Test_RenderData_DescriptionEqualsPageTitleIfExists()
        {
            var siteInfo = new SiteInfo();
            var file = new File()
            {
                MetaData = new FileMetaData {
                    { "description", "1" }
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Description.Should().Be("1");
        }

        [Fact]
        public void Test_RenderData_DescriptionEqualsSiteTitleIfExists()
        {
            var siteInfo = new SiteInfo()
            {
                Description = "2"
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Description.Should().Be("2");
        }

        [Fact]
        public void Test_RenderData_DescriptionEqualsNullIfPageAndSiteAreNull()
        {
            var siteInfo = new SiteInfo()
            {
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Description.Should().BeNull();
        }

        [Fact]
        public void Test_RenderData_LanguageEqualsPageTitleIfExists()
        {
            var siteInfo = new SiteInfo();
            var file = new File()
            {
                MetaData = new FileMetaData {
                    { "language", "1" }
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Language.Should().Be("1");
        }

        [Fact]
        public void Test_RenderData_LanguageEqualsSiteTitleIfExists()
        {
            var siteInfo = new SiteInfo()
            {
                Lang = "2"
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Language.Should().Be("2");
        }

        [Fact]
        public void Test_RenderData_LanguageEqualsNullIfPageAndSiteAreNull()
        {
            var siteInfo = new SiteInfo()
            {
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Language.Should().BeNull();
        }

        [Fact]
        public void Test_RenderData_AuthorEqualsPageTitleIfExists()
        {
            var siteInfo = new SiteInfo();
            var file = new File()
            {
                MetaData = new FileMetaData {
                    { "author", "1" }
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Author.Should().Be("1");
        }

        [Fact]
        public void Test_RenderData_AuthorEqualsSiteTitleIfExists()
        {
            var siteInfo = new SiteInfo()
            {
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            sut.Author.Should().Be(string.Empty);
        }

        [Fact]
        public void Test_RenderData_AuthorEqualsNullIfPageAndSiteAreNull()
        {
            var siteInfo = new SiteInfo()
            {
            };
            var file = new File()
            {
                MetaData = new FileMetaData
                {
                }

            };
            var siteData = new SiteData(siteInfo, new File[] { file });
            var pageData = new PageData(file);
            var sut = new RenderData { Site = siteData, Page = pageData };
            sut.Should().NotBeNull();
            // sut.Author.Should().BeNull();
            sut.Author.Should().Be(string.Empty);

        }
    }
}