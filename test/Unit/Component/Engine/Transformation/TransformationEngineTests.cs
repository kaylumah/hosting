// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using FluentAssertions;
using Kaylumah.Ssg.Engine.Transformation.Hosting;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Unit.Mocks;
using Xunit;

namespace Test.Unit;

public class TransformationEngineTests
{
    [Fact]
    public void Test1()
    {
        var fileSystemMock = new FileSystemMock();
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddTransformationEngine(configuration)
            .AddSingleton(fileSystemMock.Object)
            .AddSingleton<IMetadataProvider>(metadataProviderMock)
            .BuildServiceProvider();
        var transformationEngine = serviceProvider.GetRequiredService<ITransformationEngine>();
    }

    [Fact]
    public async Task Test_SeoPlugin_WithoutUsingResultsInEmptyString()
    {
        var fileSystemMock = new FileSystemMock();
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddTransformationEngine(configuration)
            .AddSingleton(fileSystemMock.Object)
            .AddSingleton<IMetadataProvider>(metadataProviderMock)
            .BuildServiceProvider();
        var engine = serviceProvider.GetRequiredService<ITransformationEngine>();

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


    //[Fact(Skip = "Revisit amount of tags")]
    //public async Task Test_SeoPlugin_ResultsInEmptyTags()
    //{
    //    var fileSystemMock = new FileSystemMock();
    //    var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
    //    var pluginUnderTest = new SeoPlugin();
    //    var configuration = new ConfigurationBuilder().Build();
    //    var serviceProvider = new ServiceCollection()
    //        .AddTransient<IPlugin>(_ => pluginUnderTest)
    //        .AddTransformationEngine(configuration)
    //        .AddSingleton(fileSystemMock.Object)
    //        .AddSingleton<IMetadataProvider>(metadataProviderMock)
    //        .BuildServiceProvider();
    //    var engine = serviceProvider.GetRequiredService<ITransformationEngine>();

    //    var model = new Mock<RenderData>();
    //    model.Setup(x => x.Content).Returns($"{{{{ {pluginUnderTest.Name} }}}}");
    //    var renderResult = await engine.Render(new MetadataRenderRequest[] {
    //            new MetadataRenderRequest {
    //                Metadata = model.Object
    //            }
    //        });
    //    renderResult.Should().NotBeNull();
    //    renderResult.Length.Should().Be(1);

    //    var renderContent = renderResult[0].Content;
    //    renderContent.Should().NotBeEmpty();

    //    var document = new HtmlDocument();
    //    document.LoadHtml(renderContent);
    //    var metaTags = document.DocumentNode.SelectNodes("meta");
    //    metaTags.Count.Should().Be(3);
    //}

    //[Fact]
    //public async Task Test_FeedPlugin_WithoutUsingResultsInEmptyString()
    //{
    //    var fileSystemMock = new FileSystemMock();
    //    var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());

    //    var configuration = new ConfigurationBuilder().Build();
    //    var serviceProvider = new ServiceCollection()
    //        .AddTransient<IPlugin, SeoPlugin>()
    //        .AddTransformationEngine(configuration)
    //        .AddSingleton(fileSystemMock.Object)
    //        .AddSingleton<IMetadataProvider>(metadataProviderMock)
    //        .BuildServiceProvider();
    //    var engine = serviceProvider.GetRequiredService<ITransformationEngine>();

    //    var model = new Mock<RenderData>();
    //    var renderResult = await engine.Render(new MetadataRenderRequest[] {
    //            new MetadataRenderRequest {
    //                Metadata = model.Object
    //            }
    //        });
    //    renderResult.Should().NotBeNull();
    //    renderResult.Length.Should().Be(1);

    //    var renderContent = renderResult[0].Content;
    //    renderContent.Should().BeEmpty();
    //}


    //[Fact]
    //public async Task Test_FeedPlugin_ResultsInEmptyTags()
    //{
    //    var fileSystemMock = new FileSystemMock();
    //    var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
    //    var pluginUnderTest = new SeoPlugin();
    //    var configuration = new ConfigurationBuilder().Build();
    //    var serviceProvider = new ServiceCollection()
    //        .AddTransient<IPlugin>(_ => pluginUnderTest)
    //        .AddTransformationEngine(configuration)
    //        .AddSingleton(fileSystemMock.Object)
    //        .AddSingleton<IMetadataProvider>(metadataProviderMock)
    //        .BuildServiceProvider();
    //    var engine = serviceProvider.GetRequiredService<ITransformationEngine>();

    //    var file = new Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File()
    //    {
    //        MetaData = new FileMetaData
    //        {
    //        }

    //    };
    //    var PageMetaData = file.ToPage();
    //    PageMetaData.Content = $"{{{{ {pluginUnderTest.Name} }}}}";

    //    var model = new RenderData()
    //    {
    //        Page = PageMetaData
    //    };
    //    var renderResult = await engine.Render(new MetadataRenderRequest[] {
    //            new MetadataRenderRequest {
    //                Metadata = model
    //            }
    //        });
    //    renderResult.Should().NotBeNull();
    //    renderResult.Length.Should().Be(1);

    //    var renderContent = renderResult[0].Content;
    //    renderContent.Should().NotBeEmpty();
    //}
}
