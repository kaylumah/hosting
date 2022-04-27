// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using FluentAssertions;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Xunit;

namespace Test.Unit;

public class RenderDataTests
{
    // [Fact]
    // public void TestRenderData()
    // {
    //     var siteInfo = new SiteInfo();
    //     var file = new File() {
    //         MetaData = new FileMetaData {}
    //     };
    //     var SiteMetaData = new SiteMetaData(siteInfo, new File[] { file });
    //     var PageMetaData = file.ToPage();
    //     var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
    //     sut.Should().NotBeNull();
    // }

    [Fact]
    public void Test_RenderData_ContentEqualsPageContentIfNotNull()
    {
        var siteInfo = new SiteInfo();
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            },
            Content = "-"
        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Content.Should().Be(file.Content);
    }

    [Fact]
    public void Test_RenderData_ContentEqualsEmptyStringWhenPageContentIsNull()
    {
        var siteInfo = new SiteInfo();
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            },
            Content = null
        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Content.Should().Be(string.Empty);
    }

    [Fact]
    public void Test_RenderData_TitleEqualsPageTitleIfExists()
    {
        var siteInfo = new SiteInfo();
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData {
                    { "title", "1" }
                }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Title.Should().Be("1");
    }

    [Fact]
    public void Test_RenderData_TitleEqualsSiteTitleIfExists()
    {
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData })
        { 
            Title = "2"
        };
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Title.Should().Be("2");
    }

    [Fact]
    public void Test_RenderData_TitleEqualsNullIfPageAndSiteAreNull()
    {
        var siteInfo = new SiteInfo()
        {
        };
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Title.Should().BeNull();
    }

    [Fact]
    public void Test_RenderData_DescriptionEqualsPageTitleIfExists()
    {
        var siteInfo = new SiteInfo();
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData {
                    { "description", "1" }
                }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Description.Should().Be("1");
    }

    [Fact]
    public void Test_RenderData_DescriptionEqualsSiteTitleIfExists()
    {
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData })
        {
            Description = "2"
        };
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Description.Should().Be("2");
    }

    [Fact]
    public void Test_RenderData_DescriptionEqualsNullIfPageAndSiteAreNull()
    {
        var siteInfo = new SiteInfo()
        {
        };
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Description.Should().BeNull();
    }

    [Fact]
    public void Test_RenderData_LanguageEqualsPageTitleIfExists()
    {
        var siteInfo = new SiteInfo();
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData {
                    { "language", "1" }
                }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Language.Should().Be("1");
    }

    [Fact]
    public void Test_RenderData_LanguageEqualsSiteTitleIfExists()
    {
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData })
        {
            Language = "2"
        };
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Language.Should().Be("2");
    }

    [Fact]
    public void Test_RenderData_LanguageEqualsNullIfPageAndSiteAreNull()
    {
        var siteInfo = new SiteInfo()
        {
        };
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Language.Should().BeNull();
    }

    [Fact]
    public void Test_RenderData_AuthorEqualsPageTitleIfExists()
    {
        var siteInfo = new SiteInfo();
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData {
                    { "author", "1" }
                }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Author.Should().Be("1");
    }

    [Fact]
    public void Test_RenderData_AuthorEqualsSiteTitleIfExists()
    {
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Author.Should().BeNull();
    }

    [Fact]
    public void Test_RenderData_AuthorEqualsNullIfPageAndSiteAreNull()
    {
        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        var SiteMetaData = new SiteMetaData(new PageMetaData[] { PageMetaData });
        var sut = new RenderData { Site = SiteMetaData, Page = PageMetaData };
        sut.Should().NotBeNull();
        sut.Author.Should().BeNull();
    }
}