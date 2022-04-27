// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

using FluentAssertions;
using HtmlAgilityPack;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Engine.Transformation.Service.Plugins;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Moq;
using Test.Unit.Mocks;
using Xunit;

namespace Test.Unit;

public class TransformationEngineTests
{
    [Fact]
    public void Test1()
    {
        var fileSystemMock = new FileSystemMock();
        ITransformationEngine transformEngine = new TransformationEngine(fileSystemMock.Object, new IPlugin[] { });
    }

    [Fact]
    public async Task Test_SeoPlugin_WithoutUsingResultsInEmptyString()
    {
        var pluginUnderTest = new SeoPlugin();
        var fileSystemMock = new FileSystemMock();
        var engine = new TransformationEngine(fileSystemMock.Object, new IPlugin[] {
                pluginUnderTest
            });

        var model = new Mock<RenderData>();
        var renderResult = await engine.Render(new MetadataRenderRequest[] {
                new MetadataRenderRequest {
                    Metadata = model.Object
                }
            });
        renderResult.Should().NotBeNull();
        renderResult.Length.Should().Be(1);

        var renderContent = renderResult[0].Content;
        renderContent.Should().BeEmpty();
    }


    [Fact(Skip = "Revisit amount of tags")]
    public async Task Test_SeoPlugin_ResultsInEmptyTags()
    {
        var pluginUnderTest = new SeoPlugin();
        var fileSystemMock = new FileSystemMock();
        var engine = new TransformationEngine(fileSystemMock.Object, new IPlugin[] {
                pluginUnderTest
            });

        var model = new Mock<RenderData>();
        model.Setup(x => x.Content).Returns($"{{{{ {pluginUnderTest.Name} }}}}");
        var renderResult = await engine.Render(new MetadataRenderRequest[] {
                new MetadataRenderRequest {
                    Metadata = model.Object
                }
            });
        renderResult.Should().NotBeNull();
        renderResult.Length.Should().Be(1);

        var renderContent = renderResult[0].Content;
        renderContent.Should().NotBeEmpty();

        var document = new HtmlDocument();
        document.LoadHtml(renderContent);
        var metaTags = document.DocumentNode.SelectNodes("meta");
        metaTags.Count.Should().Be(3);
    }

    [Fact]
    public async Task Test_FeedPlugin_WithoutUsingResultsInEmptyString()
    {
        var pluginUnderTest = new FeedPlugin();
        var fileSystemMock = new FileSystemMock();
        var engine = new TransformationEngine(fileSystemMock.Object, new IPlugin[] {
                pluginUnderTest
            });

        var model = new Mock<RenderData>();
        var renderResult = await engine.Render(new MetadataRenderRequest[] {
                new MetadataRenderRequest {
                    Metadata = model.Object
                }
            });
        renderResult.Should().NotBeNull();
        renderResult.Length.Should().Be(1);

        var renderContent = renderResult[0].Content;
        renderContent.Should().BeEmpty();
    }


    [Fact]
    public async Task Test_FeedPlugin_ResultsInEmptyTags()
    {
        var pluginUnderTest = new FeedPlugin();
        var fileSystemMock = new FileSystemMock();
        var engine = new TransformationEngine(fileSystemMock.Object, new IPlugin[] {
                pluginUnderTest
            });

        var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
        {
            MetaData = new FileMetaData
            {
            }

        };
        var PageMetaData = file.ToPage();
        PageMetaData.Content = $"{{{{ {pluginUnderTest.Name} }}}}";

        var model = new RenderData()
        {
            Page = PageMetaData
        };
        var renderResult = await engine.Render(new MetadataRenderRequest[] {
                new MetadataRenderRequest {
                    Metadata = model
                }
            });
        renderResult.Should().NotBeNull();
        renderResult.Length.Should().Be(1);

        var renderContent = renderResult[0].Content;
        renderContent.Should().NotBeEmpty();
    }
}