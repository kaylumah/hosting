// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Engine.Transformation.Hosting;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Hosting;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Utilities.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Test.Unit.Mocks;
using Xunit;

namespace Test.Unit;

public class SiteManagerTests
{
    [Fact]
    public async Task Test_SiteManager_GenerateSite()
    {
        var fileProcessorMock = new FileProcessorMock();
        var artifactAccessMock = new ArtifactAccessMock();
        var fileSystemMock = new FileSystemMock();
        var yamlParserMock = new YamlParserMock();
        var transformEngineMock = new Mock<ITransformationEngine>();


        var rootDirectory = Path.Combine(Environment.CurrentDirectory);

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string> {
                    { $"{nameof(SiteConfiguration)}:Source", "_site" },
                    { $"{nameof(SiteConfiguration)}:Destination", "dist" },
                    { $"{nameof(SiteConfiguration)}:LayoutDirectory", "_layouts" },
                    { $"{nameof(SiteConfiguration)}:PartialsDirectory", "_includes" },
                    { $"{nameof(SiteConfiguration)}:DataDirectory", "_data" },
                    { $"{nameof(SiteConfiguration)}:AssetDirectory", "assets" }
            });

        var configuration = configurationBuilder.Build();
        var serviceProvider = new ServiceCollection()
            .AddFileSystem()
            .AddArtifactAccess(configuration)
            .AddTransformationEngine(configuration)
            .AddSiteManager(configuration)
            .AddSingleton(fileProcessorMock.Object)
            .AddSingleton(artifactAccessMock.Object)
            .AddSingleton(fileSystemMock.Object)
            .AddSingleton(yamlParserMock.Object)
            .AddSingleton(transformEngineMock.Object)
            .Configure<SiteInfo>(_ =>
            {
                _.Url = "https://example.com";
            })
            .BuildServiceProvider();
        var siteManager = serviceProvider.GetService<ISiteManager>();
        await siteManager.GenerateSite(new GenerateSiteRequest
        {
            Configuration = new SiteConfiguration
            {
                DataDirectory = "_data",
                AssetDirectory = "assets"
            }
        });
    }
}
