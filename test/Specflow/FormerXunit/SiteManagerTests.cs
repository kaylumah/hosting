// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Hosting;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Xunit;

namespace Test.Specflow.FormerXunit;

public class SiteManagerTests
{
    [Fact(Skip = "investigae")]
    public async Task Test_SiteManager_GenerateSite()
    {
        var fileProcessorMock = new Mock<IFileProcessor>();
        var artifactAccessMock = new Mock<IArtifactAccess>();
        var fileSystemMock = new MockFileSystem();
        fileSystemMock.Directory.CreateDirectory("_site");
        fileSystemMock.Directory.CreateDirectory(Path.Combine("_site", "_data"));
        fileSystemMock.Directory.CreateDirectory(Path.Combine("_site", "assets"));
        fileSystemMock.Directory.CreateDirectory(Path.Combine("_site", "_layouts"));

        var yamlParserMock = new Mock<IYamlParser>();
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Site:Lang"] = null
        });
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Metadata:ExtensionMapping"] = null
        });
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
            .AddSiteManager(configuration)
            .AddSingleton(fileProcessorMock.Object)
            .AddSingleton(artifactAccessMock.Object)
            .AddSingleton<IFileSystem>(fileSystemMock)
            .AddSingleton(yamlParserMock.Object)
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
                Source = "_site",
                DataDirectory = "_data",
                AssetDirectory = "assets",
                LayoutDirectory = "_layouts"
            }
        });
    }
}
